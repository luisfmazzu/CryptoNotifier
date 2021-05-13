using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoobsMuc.Coinmarketcap.Client;

namespace CryptoNotifier.Services
{
    public class CryptoAnalyzer : ICryptoAnalyzer
    {
        private ICoinmarketcapClient _client;
        public void InitializeClient()
        {
            // Test
            _client = new CoinmarketcapClient("acb10e12-e8af-4251-8e68-70df0852289b");
            IEnumerable<Currency> currencies = _client.GetCurrencies(3000, "USD");
        }
    }
}
