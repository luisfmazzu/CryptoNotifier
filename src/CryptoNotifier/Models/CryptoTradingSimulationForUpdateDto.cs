using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoNotifier.Models
{
    public class CryptoTradingSimulationForUpdateDto
    {
        public DateTime? TenPercentGainDateTime { get; set; }
        public DateTime? TwentyPercentGainDateTime { get; set; }
        public DateTime? ThirtyPercentGainDateTime { get; set; }
        public double? PriceAfterOneMonth { get; set; }
        public bool IsSimulating { get; set; }
    }
}
