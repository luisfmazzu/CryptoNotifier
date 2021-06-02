using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoNotifier.Models
{
    public class CryptoTradingSimulationForCreationDto
    {
        public string SimulationId { get; set; }
        public string CryptoId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public double EntryPrice { get; set; }
        public DateTime EntryDateTime { get; set; }
        public DateTime? TenPercentGainDateTime { get; set; }
        public DateTime? TwentyPercentGainDateTime { get; set; }
        public DateTime? ThirtyPercentGainDateTime { get; set; }
        public double? PriceAfterOneMonth { get; set; }
        public bool IsSimulating { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
