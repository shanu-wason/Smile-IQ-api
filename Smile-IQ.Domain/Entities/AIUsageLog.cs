using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Domain.Entities
{
    public class AIUsageLog
    {
        public int Id { get; set; }

        public string ModelUsed { get; set; } = default!;

        public int TokensUsed { get; set; }

        public int ProcessingTimeMs { get; set; }

        public decimal CostEstimate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
