using Smile_IQ.Application.DTOs;
using Smile_IQ.Application.Interfaces;
using Smile_IQ.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.Services
{
    public class SmileScanService : ISmileScanService
    {
        private readonly ISmileScanRepository _smileRepository;
        private readonly SmileScoreCalculator _scoreCalculator;
        private readonly IOpenAIService _openAIService;
        private readonly SupabaseStorageService _storageService;

        public SmileScanService(ISmileScanRepository repository, SmileScoreCalculator calculator, IOpenAIService openAIService, SupabaseStorageService storageService)
        {
            _smileRepository = repository;
            _scoreCalculator = calculator;
            _openAIService = openAIService;
            _storageService = storageService;
        }

        public async Task<DTOSmileScanResponse> CreateAsync(DTOCreateSmileScanRequest request)
        {
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await request.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
            var mimeType = request.Image.ContentType ?? "image/jpeg";

            var imageUrl = await _storageService.UploadAsync(imageBytes, request.Image.FileName);
            var analysis = await _openAIService.AnalyzeSmileAsync(imageBytes, mimeType);

            var scoreResult = _scoreCalculator.Calculate(
                analysis.AlignmentScore,
                analysis.GumHealthScore,
                analysis.WhitenessScore,
                analysis.SymmetryScore,
                analysis.PlaqueRiskLevel);

            var scan = new SmileScan
            {
                ExternalPatientId = request.ExternalPatientId,
                ImageUrl = imageUrl,
                AlignmentScore = analysis.AlignmentScore,
                GumHealthScore = analysis.GumHealthScore,
                WhitenessScore = analysis.WhitenessScore,
                SymmetryScore = analysis.SymmetryScore,
                PlaqueRiskLevel = analysis.PlaqueRiskLevel,
                SmileScore = scoreResult.FinalScore,
                ConfidenceScore = scoreResult.ConfidenceScore,
                CreatedAt = DateTime.UtcNow
            };

            await _smileRepository.AddAsync(scan);
            await _smileRepository.SaveChangesAsync();

            return new DTOSmileScanResponse
            {
                SmileScore = scan.SmileScore,
                AlignmentScore = scan.AlignmentScore,
                GumHealthScore = scan.GumHealthScore,
                WhitenessScore = scan.WhitenessScore,
                SymmetryScore = scan.SymmetryScore,
                PlaqueRiskLevel = scan.PlaqueRiskLevel!,
                ConfidenceScore = scan.ConfidenceScore,
                Recommendations = analysis.Recommendations
            };
        }

        public async Task<List<DTOSmileScanResponse>> GetByExternalPatientIdAsync(int externalPatientId)
        {
            var scans = await _smileRepository.GetByExternalPatientIdAsync(externalPatientId);

            return scans.Select(scan => new DTOSmileScanResponse
            {
                SmileScore = scan.SmileScore,
                AlignmentScore = scan.AlignmentScore,
                GumHealthScore = scan.GumHealthScore,
                WhitenessScore = scan.WhitenessScore,
                SymmetryScore = scan.SymmetryScore,
                PlaqueRiskLevel = scan.PlaqueRiskLevel!,
                ConfidenceScore = scan.ConfidenceScore * 100
            }).ToList();
        }

        private static List<string> GetFinalRecommendation(DTOSmileAnalysisResult analysis)
        {
            var finalRecommendations = new List<string>();

            if (analysis.Recommendations != null)
                finalRecommendations.AddRange(analysis.Recommendations);

            if (analysis.PlaqueRiskLevel == "High")
                finalRecommendations.Add("Professional cleaning recommended");

            if (analysis.AlignmentScore < 60)
                finalRecommendations.Add("Orthodontic consultation suggested");

            if (analysis.GumHealthScore < 60)
                finalRecommendations.Add("Gum health evaluation recommended");

            finalRecommendations = finalRecommendations
                .Distinct()
                .ToList();
            return finalRecommendations;
        }
    }
}
