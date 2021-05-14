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
            ICoinmarketcapClient client = new CoinmarketcapClient("acb10e12-e8af-4251-8e68-70df0852289b");
            IMailService mailService = new MailService();

            while (true)
            {
                IEnumerable<Currency> currencies = client.GetCurrencies(3000, "USD");

                // Algorithm for identifying potential data
                List<Currency> currenciesToNotify = FindPotentialCoinsInBullishTrend(currencies);

                // Verify if any found coin has already been notified in the last 24h
                //TODO

                if(currenciesToNotify.Count > 0)
                {
                    string mailMessage = AssembleMailNotification(currenciesToNotify);
                    //mailService.Send("Irmãos ao Crypto - Bullish coins", mailMessage);
                }

                Thread.Sleep(1800000);
            }
        }

        private static List<Currency> FindPotentialCoinsInBullishTrend(IEnumerable<Currency> currencies)
        {
            List<Currency> bullishCoins = new List<Currency>();

            foreach(Currency coin in currencies)
            {
                double expectedPercentChange24h = 0;
                double maxPercentChange7d = 0;
                double maxPercentChange30d = 0;

                // Algorithm depends on market cap
                // Market cap > 1.000.000.000
                if (coin.MarketCapUsd > 1000000000)
                {
                    expectedPercentChange24h = 25;
                    maxPercentChange7d = 20;
                    maxPercentChange30d = 25;
                }
                else if (coin.MarketCapUsd > 100000000)
                {
                    expectedPercentChange24h = 35;
                    maxPercentChange7d = 25;
                    maxPercentChange30d = 30;
                }
                else if (coin.MarketCapUsd > 5000000)
                {
                    expectedPercentChange24h = 60;
                    maxPercentChange7d = 30;
                    maxPercentChange30d = 35;
                }
                // Less than 5.000.000
                else 
                {
                    expectedPercentChange24h = 80;
                    maxPercentChange7d = 40;
                    maxPercentChange30d = 40;
                }

                // Condition 1: Percent change of last 24h is higher then predefined values
                // Condition 2: The percent change of the last 7 days must be low to indicate there was not much movement at the market for that coin (negative or positive)
                // Condition 2: The percent change of the last month must not be too negative in order to not indicate high manipulation
                if (coin.PercentChange24h > expectedPercentChange24h &&
                    Math.Abs(coin.PercentChange7d) < maxPercentChange7d &&
                    Math.Abs(coin.PercentChange30d) < maxPercentChange30d)
                {
                    bullishCoins.Add(coin);
                }
            }

            return bullishCoins;
        }

        private static string AssembleMailNotification(List<Currency> currenciesToNotify)
        {
            string mailNotification = "";

            return mailNotification;
        }
    }
}
