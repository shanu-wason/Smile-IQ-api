using Smile_IQ.Application.DTOs;
using Smile_IQ.Application.Interfaces;
using Smile_IQ.Domain.Entities;
 

namespace Smile_IQ.Application.Services
{
    public class SmileScoreCalculator
    {
        private const decimal AlignmentWeight = 0.25m;
        private const decimal GumHealthWeight = 0.20m;
        private const decimal WhitenessWeight = 0.15m;
        private const decimal SymmetryWeight = 0.20m;

        public DTOSmileScoreResult Calculate(
            int alignment,
            int gumHealth,
            int whiteness,
            int symmetry,
            string plaqueRiskLevel)
        {
            decimal baseScore =
                (alignment * AlignmentWeight) +
                (gumHealth * GumHealthWeight) +
                (whiteness * WhitenessWeight) +
                (symmetry * SymmetryWeight);

            int plaquePenalty = plaqueRiskLevel switch
            {
                "Low" => 0,
                "Medium" => 5,
                "High" => 10,
                _ => 0
            };

            decimal finalScore = baseScore - plaquePenalty;

            if (finalScore < 0)
                finalScore = 0;

            if (finalScore > 100)
                finalScore = 100;

            // Confidence = average of inputs / 100
            decimal confidence =
                (alignment + gumHealth + whiteness + symmetry) / 400m;

            return new DTOSmileScoreResult
            {
                FinalScore = (int)Math.Round(finalScore),
                ConfidenceScore = Math.Round(confidence, 2)
            };
        }
    }
}
