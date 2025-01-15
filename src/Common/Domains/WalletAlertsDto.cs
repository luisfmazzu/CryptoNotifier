using System;
using System.Collections.Generic;

namespace Common.Domains
{
    public class WalletAlertsDto
    {
        public string Name {  get; set; }
        public List<double> TriggeredMinimumPriceAlerts { get; set; }
        public List<double> TriggeredPositivePriceAlerts { get; set; }
        public List<string> LastStates { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
    }
}
