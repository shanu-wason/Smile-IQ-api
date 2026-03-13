using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using Polly;
using Polly.Retry;
using Smile_IQ.Application.DTOs;
using Smile_IQ.Application.Exceptions;
using Smile_IQ.Application.Interfaces;
using Smile_IQ.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Smile_IQ.Infrastructure.AI
{ 
    public class OpenAIService : IOpenAIService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly AsyncRetryPolicy _retryPolicy =
                            Policy
                                .Handle<Exception>()
                                .WaitAndRetryAsync(
                                    2,
                                    retryAttempt => TimeSpan.FromSeconds(2));
        private readonly AsyncPolicy _timeoutPolicy =
                            Policy.TimeoutAsync(TimeSpan.FromSeconds(15));

        public OpenAIService(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<DTOSmileAnalysisResult> AnalyzeSmileAsync(byte[] imageBytes, string mimeType)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("OpenAI API Key not configured.");

            if (string.IsNullOrEmpty(mimeType) || !mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                mimeType = "image/jpeg";

            var client = new OpenAIClient(apiKey);
            var chatClient = client.GetChatClient("gpt-4o-mini");
            var startTime = DateTime.UtcNow;

            // Single call: either NOT_DENTAL_IMAGE or full analysis JSON
            const string systemPrompt = @"You are a dental AI. Look at the image.

            1) If the image does NOT show teeth, gums, or a smile with visible teeth (e.g. shows a bicycle, car, object, landscape, face without teeth), return ONLY this JSON and nothing else:
            { ""error"": ""NOT_DENTAL_IMAGE"", ""message"": ""Image does not contain dental imagery"" }

            2) If the image DOES show teeth/gums/smile, return ONLY this JSON format (no other text):
            { ""smileScore"": 0-100, ""alignmentScore"": 0-100, ""gumHealthScore"": 0-100, ""whitenessScore"": 0-100, ""symmetryScore"": 0-100, ""plaqueRiskLevel"": ""Low"" or ""Medium"" or ""High"", ""confidenceScore"": 0.0-1.0, ""recommendations"": [""string""] }

            Rules: Return only valid JSON. No markdown. Never invent dental scores for non-dental images.";
            var userMessage = new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Look at the image. Return the appropriate JSON only."),
                ChatMessageContentPart.CreateImagePart(new BinaryData(imageBytes), mimeType));

            var response = await _retryPolicy.WrapAsync(_timeoutPolicy)
                .ExecuteAsync(async () =>
                    await chatClient.CompleteChatAsync(
                        new ChatMessage[] { ChatMessage.CreateSystemMessage(systemPrompt), userMessage }));

            var processingTime = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
            var content = response.Value.Content[0].Text ?? "";

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("AI returned empty response.");

            if (content.Contains("\"error\"", StringComparison.OrdinalIgnoreCase) &&
                content.Contains("NOT_DENTAL_IMAGE", StringComparison.OrdinalIgnoreCase))
                throw new ImageValidationException();

            var result = JsonSerializer.Deserialize<DTOSmileAnalysisResult>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                throw new Exception("AI returned invalid JSON.");

            ValidateAnalysisResult(result);

            int tokensUsed = response.Value.Usage != null
                ? response.Value.Usage.InputTokenCount + response.Value.Usage.OutputTokenCount
                : 0;
            await LogUsageAsync(
                model: "gpt-4o-mini",
                tokens: tokensUsed,
                processingTime: processingTime,
                cost: tokensUsed * 0.00000015m
            );

            return result;
        }

        private static void ValidateAnalysisResult(DTOSmileAnalysisResult result)
        {
            if (result == null)
                throw new Exception("AI returned null result.");

            // Score range validation
            ValidateScore(result.AlignmentScore, nameof(result.AlignmentScore));
            ValidateScore(result.GumHealthScore, nameof(result.GumHealthScore));
            ValidateScore(result.WhitenessScore, nameof(result.WhitenessScore));
            ValidateScore(result.SymmetryScore, nameof(result.SymmetryScore));

            // Plaque risk validation
            if (result.PlaqueRiskLevel != "Low" &&
                result.PlaqueRiskLevel != "Medium" &&
                result.PlaqueRiskLevel != "High")
            {
                throw new Exception("AI returned invalid plaqueRiskLevel.");
            }

            // Confidence validation (if AI returns it)
            if (result.ConfidenceScore < 0 || result.ConfidenceScore > 1)
            {
                throw new Exception("AI returned invalid confidenceScore.");
            }

            // Recommendations validation
            if (result.Recommendations == null)
            {
                throw new Exception("AI returned null recommendations.");
            }
        }

        private static void ValidateScore(int score, string propertyName)
        {
            if (score < 0 || score > 100)
                throw new Exception($"AI returned invalid {propertyName}.");
        }

        private async Task LogUsageAsync(
            string model,
            int tokens,
            int processingTime,
            decimal cost)
        {
            var log = new AIUsageLog
            {
                ModelUsed = model,
                TokensUsed = tokens,
                ProcessingTimeMs = processingTime,
                CostEstimate = cost,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.AIUsageLog.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
