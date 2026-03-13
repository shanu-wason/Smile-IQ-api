using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Domain.Entities
{
    public class SmileScan
    {
        public int Id { get; set; }

        public int ExternalPatientId { get; set; }

        public string? ImageUrl { get; set; }

        public int SmileScore { get; set; }

        public int AlignmentScore { get; set; }

        public int GumHealthScore { get; set; }

        public int WhitenessScore { get; set; }

        public int SymmetryScore { get; set; }

        public string? PlaqueRiskLevel { get; set; }

        public decimal ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
