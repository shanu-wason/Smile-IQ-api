using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.DTOs
{
    public class DTOSmileScanResponse
    {
        public int SmileScore { get; set; }
        public int AlignmentScore { get; set; }
        public int GumHealthScore { get; set; }
        public int WhitenessScore { get; set; }
        public int SymmetryScore { get; set; }
        public string PlaqueRiskLevel { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
}
