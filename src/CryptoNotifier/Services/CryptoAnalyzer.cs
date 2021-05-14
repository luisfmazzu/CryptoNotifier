using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NoobsMuc.Coinmarketcap.Client;

namespace CryptoNotifier.Services
{
    public class CryptoAnalyzer : ICryptoAnalyzer
    {
        public void InitializeClient()
        {
            Thread t = new Thread(AnalyzeCurrencies);
            t.Start();
        }

        static void AnalyzeCurrencies()
        {
            ICoinmarketcapClient client;
            // teste email
            IMailService mailService = new MailService();
            mailService.Send("Teste obj", "testee");
            while (true)
            {
                client = new CoinmarketcapClient("acb10e12-e8af-4251-8e68-70df0852289b");
                IEnumerable<Currency> currencies = client.GetCurrencies(3000, "USD");

                Thread.Sleep(1800000);
            }
            
        }
    }
}
