using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.DTOs
{
    public class DTOSmileScoreResult
    {
        public int FinalScore { get; set; }
        public decimal ConfidenceScore { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
}
