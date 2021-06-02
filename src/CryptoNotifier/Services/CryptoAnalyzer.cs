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
using System.Globalization;

namespace CryptoNotifier.Services
{
    public class CryptoAnalyzer : ICryptoAnalyzer
    {
        private ICryptoDataRepository _cryptoDataRepository;
        private ICryptoTradingSimulationRepository _cryptoTradingSimulationRepository;
        public void InitializeClient(ICryptoDataRepository cryptoDataRepository, ICryptoTradingSimulationRepository cryptoTradingSimulationRepository)
        {
            _cryptoDataRepository = cryptoDataRepository;
            _cryptoTradingSimulationRepository = cryptoTradingSimulationRepository;
            AnalyzeCurrencies();
        }

        private async void AnalyzeCurrencies()
        {
            ICoinmarketcapClient client = new CoinmarketcapClient("ed57fadd-e7a7-4eb8-8fd5-ab510d358856");
            IMailService mailService = new MailService();

            while (true)
            {
                DateTime currentTime = DateTime.Now;

                IEnumerable<Currency> currencies = client.GetCurrencies(1500, "USD");

                // Verify if there were any gains at crypto trading simulation
                await VerifyCryptoTradingSimulation(currencies);

                // Algorithm for identifying potential data
                List<Currency> currenciesToNotify = FindPotentialCoinsInBullishTrend(currencies);

                if (currenciesToNotify.Count > 0)
                {
                    // Verify if any found coin has already been notified in the last 24h
                    currenciesToNotify = await RemoveCoinsWithExistingNotification(currenciesToNotify);

                    // If there are still coins to be notified
                    if (currenciesToNotify.Count > 0)
                    {
                        // Update database with new coins for simulation
                        await AddCryptoTradingSimulationsAtDB(currenciesToNotify);

                        string mailMessage = AssembleMailNotification(currenciesToNotify);
                        mailService.Send("Irmãos ao Crypto - NEW Bullish coins", mailMessage);

                        // Update database with this notification
                        await UpdateNotificationsAtDB(currenciesToNotify);
                    }
                }

                Thread.Sleep(4000000);
            }
        }

        private async Task<List<Currency>> RemoveCoinsWithExistingNotification(List<Currency> currencies)
        {
            List<Currency> currenciesToNotify = new List<Currency>();
            foreach (Currency coin in currencies)
            {
                var coinInDB = await _cryptoDataRepository.GetCryptoDataByTickerSymbol(coin.Symbol);
                if (coinInDB != null)
                {
                    // Verify date time and if it is a shit coin (determined by the user)
                    DateTime currentTime = DateTime.Now;
                    if (((DateTime)coinInDB.LastModified - currentTime).Days >= 1 && coinInDB.ShitCoin == false)
                    {
                        currenciesToNotify.Add(coin);
                    }
                }
                else
                {
                    currenciesToNotify.Add(coin);
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
                        ShitCoin = false,
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
                        ShitCoin = false,
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

            foreach (Currency coin in currencies)
            {
                double expectedPercentChange1h = 0;
                double expectedPercentChange24h = 0;
                double maxPercentChange7d = 0;
                double maxPercentChange30d = 0;
                double minMarketCap = 1000000;

                // Algorithm depends on market cap
                // Market cap > 1.000.000.000
                if (coin.MarketCapUsd > 1000000000)
                {
                    expectedPercentChange1h = 8;
                    expectedPercentChange24h = 20;
                    maxPercentChange7d = 20;
                    maxPercentChange30d = 25;
                }
                else if (coin.MarketCapUsd > 100000000)
                {
                    expectedPercentChange1h = 13;
                    expectedPercentChange24h = 30;
                    maxPercentChange7d = 25;
                    maxPercentChange30d = 30;
                }
                else if (coin.MarketCapUsd > 5000000)
                {
                    expectedPercentChange1h = 18;
                    expectedPercentChange24h = 55;
                    maxPercentChange7d = 30;
                    maxPercentChange30d = 35;
                }
                // Less than 5.000.000 and more than minMarketCap
                else if (coin.MarketCapUsd > minMarketCap)
                {
                    expectedPercentChange1h = 30;
                    expectedPercentChange24h = 80;
                    maxPercentChange7d = 40;
                    maxPercentChange30d = 40;
                }

                bool coinHasBeenAlreadyAdded = false;
                // Algorithm 1
                // Condition 1: Percent change of last 24h is higher then predefined values
                // Condition 2: The percent change of the last 7 days must be low to indicate there was not much movement at the market for that coin (negative or positive)
                // Condition 3: The percent change of the last month must not be too negative in order to not indicate high manipulation
                // Condition 4: Market cap is not too low
                if (coin.PercentChange24h > expectedPercentChange24h &&
                    Math.Abs(coin.PercentChange7d) < maxPercentChange7d &&
                    Math.Abs(coin.PercentChange30d) < maxPercentChange30d &&
                    coin.MarketCapUsd > minMarketCap)
                {
                    bullishCoins.Add(coin);
                    coinHasBeenAlreadyAdded = true;
                }

                // Algorithm 2
                // Condition 1: Percent change of last 1h is higher then predefined values
                // Condition 2: The percent change of the last 7 days must be low to indicate there was not much movement at the market for that coin (negative or positive)
                // Condition 3: The percent change of the last month must not be too negative in order to not indicate high manipulation
                // Condition 4: Coin has not been added already
                // Condition 5: Market cap is not too low
                if (coin.PercentChange1h >= expectedPercentChange1h &&
                    Math.Abs(coin.PercentChange7d) <= maxPercentChange7d &&
                    Math.Abs(coin.PercentChange30d) <= maxPercentChange30d &&
                    !coinHasBeenAlreadyAdded &&
                    coin.MarketCapUsd > minMarketCap)
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

            foreach (Currency coin in currenciesToNotify)
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

        private async Task AddCryptoTradingSimulationsAtDB(List<Currency> newCurrencies)
        {
            foreach(Currency coin in newCurrencies)
            {
                DateTime currentTime = DateTime.Now;
                var simulationId = Guid.NewGuid();
                CryptoTradingSimulationForCreationDto cryptoTradingSimulation = new CryptoTradingSimulationForCreationDto
                {
                    SimulationId = simulationId.ToString(),
                    CryptoId = coin.Id,
                    Name = coin.Name,
                    Symbol = coin.Symbol,
                    EntryPrice = coin.Price,
                    EntryDateTime = currentTime,
                    IsSimulating = true,
                    CreatedOn = currentTime,
                    LastModified = currentTime
                };

                var coinInDBToBeCreated = Startup.Mapper.Map<ICryptoTradingSimulationDomain>(cryptoTradingSimulation);

                await _cryptoTradingSimulationRepository.InsertCryptoTradingSimulation(coinInDBToBeCreated);
            }
        }

        private async Task VerifyCryptoTradingSimulation(IEnumerable<Currency> currencies)
        {
            var functionalTradingSimulations = await _cryptoTradingSimulationRepository.GetAllFunctionalCryptoTradingSimulation();
            DateTime currentTime = DateTime.Now;

            foreach (var tradingSimulation in functionalTradingSimulations)
            {
                var currency = currencies.Where(x => x.Id == tradingSimulation.CryptoId).First();

                if(currency.Price >= tradingSimulation.EntryPrice * 1.1)
                {
                    var tradingSimulationForUpdate = new CryptoTradingSimulationForUpdateDto()
                    {
                        TenPercentGainDateTime = tradingSimulation.TenPercentGainDateTime,
                        TwentyPercentGainDateTime = tradingSimulation.TwentyPercentGainDateTime,
                        ThirtyPercentGainDateTime = tradingSimulation.ThirtyPercentGainDateTime,
                        IsSimulating = tradingSimulation.IsSimulating,
                        PriceAfterOneMonth = tradingSimulation.PriceAfterOneMonth
                    };
                    bool update = false;

                    if (tradingSimulation.TenPercentGainDateTime != null)
                    {
                        tradingSimulationForUpdate.TenPercentGainDateTime = currentTime;
                        update = true;
                    }
                    if(currency.Price >= tradingSimulation.EntryPrice * 1.2 && tradingSimulation.TwentyPercentGainDateTime != null)
                    {
                        tradingSimulationForUpdate.TwentyPercentGainDateTime = currentTime;
                        update = true;
                    }
                    if (currency.Price >= tradingSimulation.EntryPrice * 1.3 && tradingSimulation.ThirtyPercentGainDateTime != null)
                    {
                        tradingSimulationForUpdate.ThirtyPercentGainDateTime = currentTime;
                        tradingSimulationForUpdate.IsSimulating = false;
                        update = true;
                    }

                    if(update)
                    { 
                        var coinInDBToBeUpdated = Startup.Mapper.Map<ICryptoTradingSimulationDomain>(tradingSimulationForUpdate);

                        await _cryptoTradingSimulationRepository.UpdateCryptoTradingSimulation(tradingSimulation.SimulationId, coinInDBToBeUpdated);
                    }
                }
                else if((tradingSimulation.EntryDateTime - currentTime).Days > 30)
                {
                    var tradingSimulationForUpdate = new CryptoTradingSimulationForUpdateDto()
                    {
                        TenPercentGainDateTime = tradingSimulation.TenPercentGainDateTime,
                        TwentyPercentGainDateTime = tradingSimulation.TwentyPercentGainDateTime,
                        ThirtyPercentGainDateTime = tradingSimulation.ThirtyPercentGainDateTime,
                        IsSimulating = tradingSimulation.IsSimulating,
                        PriceAfterOneMonth = currency.Price
                    };

                    var coinInDBToBeUpdated = Startup.Mapper.Map<ICryptoTradingSimulationDomain>(tradingSimulationForUpdate);

                    await _cryptoTradingSimulationRepository.UpdateCryptoTradingSimulation(tradingSimulation.SimulationId, coinInDBToBeUpdated);
                }
            }
        }
    }
}
