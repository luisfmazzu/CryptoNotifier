using System;

namespace Common.Domains
{
    public class TokenHistory
    {
        public string Symbol;
        public string State;
        public string LastState;
        public bool SentSellPriceAlert;
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
    }
}
