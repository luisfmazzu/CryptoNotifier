using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domains
{
    public class ICryptoDataDomain
    {
        public string CryptoID { get; set; }
        public string CryptoTickerSymbol { get; set; }
        public string CryptoName { get; set; }
        public float? CurrentMarketCap { get; set; }
        public List<float> DailyQuotationCB { get; set; }
        public List<float> DailyQuotationPercentChangeCB { get; set; }
        public List<float> QuotationPercentChangeCB { get; set; }
        public int? QuotationPercentChangeCBIterator { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
