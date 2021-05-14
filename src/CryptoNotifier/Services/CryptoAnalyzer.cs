using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NoobsMuc.Coinmarketcap.Client;
using MongoPersistence;
using Common.Repositories;
using Common.Domains;
using CryptoNotifier.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptoNotifier.Services
{
    public class CryptoAnalyzer : ICryptoAnalyzer
    {
        private ICryptoDataRepository _cryptoDataRepository;
        public void InitializeClient(ICryptoDataRepository cryptoDataRepository)
        {
            _cryptoDataRepository = cryptoDataRepository;
            AnalyzeCurrencies();
        }

        private async void AnalyzeCurrencies()
        {
            ICoinmarketcapClient client = new CoinmarketcapClient("acb10e12-e8af-4251-8e68-70df0852289b");
            IMailService mailService = new MailService();

            while (true)
            {
                IEnumerable<Currency> currencies = client.GetCurrencies(3000, "USD");

                // Algorithm for identifying potential data
                List<Currency> currenciesToNotify = FindPotentialCoinsInBullishTrend(currencies);

                if(currenciesToNotify.Count > 0)
                {
                    // Verify if any found coin has already been notified in the last 24h
                    currenciesToNotify = await RemoveCoinsWithExistingNotification(currenciesToNotify);

                    // If there are still coins to be notified
                    if (currenciesToNotify.Count > 0)
                    {
                        string mailMessage = AssembleMailNotification(currenciesToNotify);
                        //mailService.Send("Irmãos ao Crypto - Bullish coins", mailMessage);

                        // Update database with this notification
                        await UpdateNotificationsAtDB(currenciesToNotify);
                    }
                }

                Thread.Sleep(1800000);
            }
        }

        private async Task<List<Currency>> RemoveCoinsWithExistingNotification(List<Currency> currenciesToNotify)
        {
            foreach(Currency coin in currenciesToNotify)
            {
                var coinInDB = await _cryptoDataRepository.GetCryptoDataByTickerSymbol(coin.Symbol);
                if (coinInDB != null)
                {
                    // Verify date time
                    DateTime currentTime = DateTime.Now;
                    if(((DateTime)coinInDB.LastModified - currentTime).Days < 1)
                    {
                        currenciesToNotify.Remove(coin);
                    }
                }
            }

            return currenciesToNotify;
        }

        private async Task UpdateNotificationsAtDB(List<Currency> currenciesToNotify)
        {
            foreach (Currency coin in currenciesToNotify)
            {
                var coinInDB = await _cryptoDataRepository.GetCryptoDataByTickerSymbol(coin.Symbol);

                // Create new coin
                if (coinInDB == null)
                {
                    DateTime currentTime = DateTime.Now;
                    CryptoDataForCreationDto cryptoData = new CryptoDataForCreationDto
                    {
                        CryptoId = coin.Id,
                        Name = coin.Name,
                        Symbol = coin.Symbol,
                        Rank = coin.Rank,
                        Price = coin.Price,
                        Volume24hUsd = coin.Volume24hUsd,
                        MarketCapUsd = coin.MarketCapUsd,
                        PercentChange1h = coin.PercentChange1h,
                        PercentChange24h = coin.PercentChange24h,
                        PercentChange7d = coin.PercentChange7d,
                        PercentChange30d = coin.PercentChange30d,
                        MarketCapConvert = coin.MarketCapConvert,
                        ConvertCurrency = coin.ConvertCurrency,
                        CreatedOn = currentTime,
                        LastModified = currentTime
                    };

                    var coinInDBToBeCreated = Startup.Mapper.Map<ICryptoDataDomain>(cryptoData);

                    await _cryptoDataRepository.InsertCryptoData(coinInDBToBeCreated);
                }
                // Update existing coin
                else
                {
                    DateTime currentTime = DateTime.Now;
                    CryptoDataForUpdateDto cryptoData = new CryptoDataForUpdateDto
                    {
                        Rank = coin.Rank,
                        Price = coin.Price,
                        Volume24hUsd = coin.Volume24hUsd,
                        MarketCapUsd = coin.MarketCapUsd,
                        PercentChange1h = coin.PercentChange1h,
                        PercentChange24h = coin.PercentChange24h,
                        PercentChange7d = coin.PercentChange7d,
                        PercentChange30d = coin.PercentChange30d,
                        MarketCapConvert = coin.MarketCapConvert,
                        LastModified = currentTime
                    };

                    var coinInDBToBeUpdated = Startup.Mapper.Map<ICryptoDataDomain>(cryptoData);

                    await _cryptoDataRepository.UpdateCryptoData(coin.Symbol, coinInDBToBeUpdated);
                }
            }

            return;
        }

        private List<Currency> FindPotentialCoinsInBullishTrend(IEnumerable<Currency> currencies)
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

        private string AssembleMailNotification(List<Currency> currenciesToNotify)
        {
            string mailNotification = "";

            DateTime currentTime = DateTime.Now;

            mailNotification += currentTime.ToString("F", DateTimeFormatInfo.InvariantInfo) + "<br/><br/>";
            mailNotification += "<u>PC1h = Percent change 1h // PC24h = Percent change 24h // PC7d = Percent change 7 days // PC30d = Percent Change 30 days // MC = Market cap</u> <br/><br/>";

            foreach(Currency coin in currenciesToNotify)
            {
                mailNotification += "<b>Name:</b> " + coin.Name + " || ";
                mailNotification += "<b>Symbol:</b> " + coin.Symbol + " || ";
                mailNotification += "<b>PC1h:</b> " + coin.PercentChange1h.ToString("n2") + " || ";
                mailNotification += "<b>PC24h:</b> " + coin.PercentChange24h.ToString("n2") + " || ";
                mailNotification += "<b>PC7d:</b> " + coin.PercentChange7d.ToString("n2") + " || ";
                mailNotification += "<b>PC30d:</b> " + coin.PercentChange30d.ToString("n2") + " || ";
                mailNotification += "<b>MC:</b> " + coin.MarketCapUsd.ToString("n2");
                mailNotification += "<br/>";
            }

            return mailNotification;
        }
    }
}
