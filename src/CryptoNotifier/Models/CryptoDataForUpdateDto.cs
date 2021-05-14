using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoNotifier.Models
{
    public class CryptoDataForUpdateDto
    {
        public string Symbol { get; set; }
        public string Rank { get; set; }
        public double Price { get; set; }
        public double Volume24hUsd { get; set; }
        public double MarketCapUsd { get; set; }
        public double PercentChange1h { get; set; }
        public double PercentChange24h { get; set; }
        public double PercentChange7d { get; set; }
        public double PercentChange30d { get; set; }
        public Double MarketCapConvert { get; set; }
        public bool ShitCoin { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
