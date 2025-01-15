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
using System.Net.Mail;
using MongoPersistence.MongoPersistence;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using CoinGecko.Clients;
using Newtonsoft.Json;
using System.Net.Http;
using Twilio.TwiML.Voice;
using System.Text;
using System.Xml.Linq;

namespace CryptoNotifier.Services
{
    public class CryptoAnalyzer : ICryptoAnalyzer
    {
        private ICryptoDataRepository _cryptoDataRepository;
        private IMailService _mailService;
        private IPhoneCallService _phoneCallService;
        private IWalletAlertsRepository _walletAlertsRepository;
        private ITokenHistoryRepository _tokenHistoryRepository;
        public Dictionary<string, TokenHistory> watchlistHistory = new Dictionary<string, TokenHistory>();
        public Dictionary<string, WalletAlertsDto> walletAlerts = new Dictionary<string, WalletAlertsDto>();
        public List<WalletEntity> wallets;
        public List<int> dailyWalletUpdateHours;
        public int currentHourIndex;
        public double usdToBrl;
        public int currentMoneyAlert;
        public bool allowPhoneCalls = true;

        public void InitializeClient(ICryptoDataRepository cryptoDataRepository, IMailService mailService, IPhoneCallService phoneCallService, IWalletAlertsRepository walletAlertsRepository, ITokenHistoryRepository tokenHistoryRepository)
        {
            _cryptoDataRepository = cryptoDataRepository;
            _walletAlertsRepository = walletAlertsRepository;
            _tokenHistoryRepository = tokenHistoryRepository;
            _mailService = mailService;
            _phoneCallService = phoneCallService;
            currentHourIndex = -1;
            dailyWalletUpdateHours = new List<int>()
            {
                6, 13, 20
            };
            usdToBrl = 6;
            currentMoneyAlert = 0;

            InitializeWatchlistHistory();
            InitializeWalletAlerts();
            InitializeWallet();
            AnalyzeCurrencies();
        }

        private async void InitializeWalletAlerts()
        {
            var walletAlertsList = await _walletAlertsRepository.GetAllWalletAlertsDto();
            foreach (var walletAlert in walletAlertsList)
            {
                walletAlert.LastStates = new List<string>();
                walletAlerts[walletAlert.Name] = walletAlert;
            }

            foreach(var wallet in wallets)
            {
                if (!walletAlerts.ContainsKey(wallet.walletName))
                {
                    WalletAlertsDto walletAlertsDto = new WalletAlertsDto()
                    {
                        Name = wallet.walletName,
                        TriggeredMinimumPriceAlerts = new List<double>(),
                        TriggeredPositivePriceAlerts = new List<double>(),
                        LastStates = new List<string>(),
                        CreatedOn = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    walletAlerts.Add(wallet.walletName, walletAlertsDto);
                    await _walletAlertsRepository.InsertWalletAlertsDto(walletAlertsDto);
                }
            }
        }

        private int getCurrentHourIndex(DateTime now)
        {
            int index = 0;

            for(int i = 0; i < dailyWalletUpdateHours.Count; i++)
            {
                if (dailyWalletUpdateHours[i] > now.Hour)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void InitializeWallet()
        {
            wallets = new List<WalletEntity>();
            WalletEntity luisWallet = Watchlist.luisWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in luisWallet.tokens)
            {
                luisWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(luisWallet);

            WalletEntity gusWallet1 = Watchlist.gusWallet1;
            foreach (KeyValuePair<string, TokenEntity> kvp in gusWallet1.tokens)
            {
                gusWallet1.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(gusWallet1);

            WalletEntity modenaWallet = Watchlist.modenaWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in modenaWallet.tokens)
            {
                modenaWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
                modenaWallet.originalMoney += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(modenaWallet);

            WalletEntity henriqueWallet1 = Watchlist.henriqueWallet1;
            foreach (KeyValuePair<string, TokenEntity> kvp in henriqueWallet1.tokens)
            {
                henriqueWallet1.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
                henriqueWallet1.originalMoney += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(henriqueWallet1);

            WalletEntity henriqueWallet2 = Watchlist.henriqueWallet2;
            foreach (KeyValuePair<string, TokenEntity> kvp in henriqueWallet2.tokens)
            {
                henriqueWallet2.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
                henriqueWallet2.originalMoney += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(henriqueWallet2);

            WalletEntity bibiWallet = Watchlist.bibiWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in bibiWallet.tokens)
            {
                bibiWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(bibiWallet);

            WalletEntity meriWallet = Watchlist.meriWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in meriWallet.tokens)
            {
                meriWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(meriWallet);

            WalletEntity alejandroWallet = Watchlist.alejandroWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in alejandroWallet.tokens)
            {
                alejandroWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
                alejandroWallet.originalMoney += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(alejandroWallet);

            WalletEntity cesarWallet = Watchlist.cesarWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in cesarWallet.tokens)
            {
                cesarWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(cesarWallet);

            WalletEntity keiaWallet = Watchlist.keiaWallet;
            foreach (KeyValuePair<string, TokenEntity> kvp in keiaWallet.tokens)
            {
                keiaWallet.totalMoneySpent += kvp.Value.averagePrice * kvp.Value.totalTokens;
            }
            wallets.Add(keiaWallet);
        }

        private async void InitializeWatchlistHistory()
        {
            var tokenHistoryList = await _tokenHistoryRepository.GetAllTokenHistoryDto();
            foreach (var tokenHistory in tokenHistoryList)
            {
                watchlistHistory[tokenHistory.Symbol] = tokenHistory;
            }
            foreach (KeyValuePair<string, FixedTokenEntity> kvp in Watchlist.fixedTokens)
            {
                if (!watchlistHistory.ContainsKey(kvp.Key))
                {
                    TokenHistory tokenHistory = new TokenHistory()
                    {
                        Symbol = kvp.Key,
                        State = Watchlist.black,
                        LastState = Watchlist.black,
                        SentSellPriceAlert = false,
                        CreatedOn = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    watchlistHistory.Add(kvp.Key, tokenHistory);
                    await _tokenHistoryRepository.InsertTokenHistory(tokenHistory);
                }
            }
            foreach (KeyValuePair<string, WatchlistEntity> kvp in Watchlist.watchlist)
            {
                if (!watchlistHistory.ContainsKey(kvp.Key))
                {
                    TokenHistory tokenHistory = new TokenHistory()
                    {
                        Symbol = kvp.Key,
                        State = Watchlist.black,
                        LastState = Watchlist.black,
                        SentSellPriceAlert = false,
                        CreatedOn = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    watchlistHistory.Add(kvp.Key, tokenHistory); 
                    await _tokenHistoryRepository.InsertTokenHistory(tokenHistory);
                }
            }
        }

        private void ResetWalletCurrentPrices()
        {
            for (int i = 0; i < wallets.Count; i++)
            {
                foreach (KeyValuePair<string, TokenEntity> kvp in wallets[i].tokens)
                {
                    wallets[i].tokens[kvp.Key].currentPrice = 0;
                }
                wallets[i].currentMoney = 0;
            }
        }

        private async System.Threading.Tasks.Task AnalyzeWallets()
        {
            DateTime now = DateTime.Now;
            Dictionary<string, string> mail = new Dictionary<string, string>();
            HashSet<string> emails;
            double priceDifference;
            double percentage;
            string price;

            foreach (WalletEntity wallet in wallets)
            {
                mail = new Dictionary<string, string>();
                emails = wallet.email;
                emails.Add("luisfmazzu@gmail.com");
                bool priceIssue = false;

                foreach (KeyValuePair<string, TokenEntity> kvp in wallet.tokens)
                {
                    if (kvp.Value.currentPrice == 0)
                    {
                        Console.WriteLine("Issue analyzing token " + kvp.Key);
                        priceIssue = true;
                        break;
                    }
                    if (!Watchlist.fixedTokens.ContainsKey(kvp.Key))
                    {
                        Console.WriteLine("Issue analyzing token " + kvp.Key);
                        break;
                    }
                    double sellPrice = Watchlist.fixedTokens[kvp.Key].sellPrice;
                    double highestPrice = Watchlist.fixedTokens[kvp.Key].highestPrice;
                    if (kvp.Value.currentPrice >= sellPrice && watchlistHistory.ContainsKey(kvp.Key) && !watchlistHistory[kvp.Key].SentSellPriceAlert)
                    {
                        double totalBuyPrice = kvp.Value.totalTokens * kvp.Value.averagePrice;
                        double totalCurrentPrice = kvp.Value.totalTokens * kvp.Value.currentPrice;
                        priceDifference = totalCurrentPrice - totalBuyPrice;
                        percentage = (priceDifference / totalBuyPrice) * 100;
                        price = "";
                        if (percentage > 0)
                        {
                            price += "+";
                        }
                        price += percentage.ToString("0.00");
                        mail["subject"] = kvp.Key;
                        mail["message"] = "The token " + kvp.Key + "has reached the sell price of " + sellPrice.ToString("0.0000") + ". If you sell now, you'll earn " + price + "% over the inital value, resulting in a total of " + priceDifference.ToString("0.00") + " USD.<br>";
                        mail["message"] += "<b>" + kvp.Key + " ATH (all time high):</b> " + highestPrice.ToString("0.0000") + "<br>";
                        mail["message"] += "<b>IMPORTANT: This is an alert for consideration. It may be worth to sell now OR not, it depends on many other factors. The token may not reach near the ATH again as well. Please consult your mega blaster crypto consultant to know what to do.<br><br>";
                        mail["message"] += "<b>Symbol:</b> " + kvp.Key.ToUpper() + " || ";
                        mail["message"] += "<b>Buy price:</b> " + kvp.Value.averagePrice.ToString() + " || ";
                        mail["message"] += "<b>Current price:</b> " + kvp.Value.currentPrice.ToString() + " || ";
                        mail["message"] += "<b>Total buy price:</b> " + (totalBuyPrice).ToString("0.00") + " || ";
                        mail["message"] += "<b>Total current price:</b> " + (totalCurrentPrice).ToString("0.00") + " || ";
                        mail["message"] += "<b>Percentage:</b> " + price + " || <br><br>";
                        watchlistHistory[kvp.Key].SentSellPriceAlert = true;
                    }
                }
                if(priceIssue)
                {
                    mail["subject"] = "Error generating" + wallet.walletName + "'s wallet update";
                    mail["message"] = "An error occured when updating your wallet. Please remain calm and don't cry. The mega blaster engineer behind this app will soon fix it.";
                    //await _mailService.SendMail(mail["subject"], mail["message"], emails);
                    continue;
                }
                if (mail.ContainsKey("subject"))
                {
                    mail["subject"] += "'s reached their sell price - Consider selling!";
                    await _mailService.SendMail(mail["subject"], mail["message"], emails);
                }

                // Check for wallet money alerts
                mail = new Dictionary<string, string>();
                emails = wallet.email;
                emails.Add("luisfmazzu@gmail.com");
                priceDifference = wallet.currentMoney - wallet.originalMoney;
                percentage = (priceDifference / wallet.originalMoney) * 100;
                string subject = "";

                if (percentage <= -30)
                {
                    subject = wallet.walletName + "'s Wallet Emergency -30% - Sell Now";
                }
                else if (percentage <= -25)
                {
                    subject = wallet.walletName + "'s Wallet Extra attention -25%";
                }
                else if (percentage <= -20)
                {
                    subject = wallet.walletName + "'s Wallet Attention -20%";
                }
                else if (percentage <= -15)
                {
                    subject = wallet.walletName + "'s Wallet Not that Great -15%";
                }
                else if (percentage <= -10)
                {
                    subject = wallet.walletName + "'s Wallet Hang on -10%";
                }
                else if (percentage >= 30)
                {
                    subject = wallet.walletName + "'s Wallet It's starting! +30%";
                }
                else if (percentage >= 50)
                {
                    subject = wallet.walletName + "'s Wallet Up Again! +50%";
                }
                else if (percentage >= 80)
                {
                    subject = wallet.walletName + "'s Wallet Almost theere! +80%";
                }
                else if (percentage >= 100)
                {
                    subject = wallet.walletName + "'s Wallet HERE IT IS +100%";
                }
                else if (percentage >= 150)
                {
                    subject = wallet.walletName + "'s Wallet EYES ARE SHINY +150%";
                }
                else if (percentage >= 200)
                {
                    subject = wallet.walletName + "'s Wallet ENJOY YOUR MONEEEY +200%";
                }
                else if (percentage >= 250)
                {
                    subject = wallet.walletName + "'s Wallet CRAZYYYYYYYYYYY +250%";
                }
                else if (percentage >= 300)
                {
                    subject = wallet.walletName + "'s Wallet OMGGGGGGGGGGGGGGGGGGGG +300%";
                }

                if (subject.Length > 0 && walletAlerts[wallet.walletName].LastStates.Contains(subject))
                {
                    mail = AdjustMailNotification(mail);

                    price = "";
                    if (priceDifference > 0)
                    {
                        price += "+";
                    }
                    price += percentage.ToString("0.00");
                    mail["subject"] = subject;
                    mail["message"] = "Wallet " + wallet.walletName + " summary: Current percentage is <b>" + price + "%</b> and current total is <b>" + wallet.currentMoney.ToString("N0") + " USD</b>. You have spent <b>" + wallet.originalMoney.ToString("N0") + " USD</b> so far.";
                    walletAlerts[wallet.walletName].LastStates.Add(subject);
                    await _mailService.SendMail(mail["subject"], mail["message"], emails);
                }

                double currentMoneyBRL = wallet.currentMoney * usdToBrl;
                if (!walletAlerts[wallet.walletName].TriggeredMinimumPriceAlerts.Contains(wallet.minimumPriceAlertBRL) && currentMoneyBRL <= wallet.minimumPriceAlertBRL && allowPhoneCalls)
                {
                    _phoneCallService.SendVoiceCall(
                        wallet.walletName,
                        wallet.currentMoney*usdToBrl,
                        wallet.telNumber,
                        false
                    );
                    walletAlerts[wallet.walletName].TriggeredMinimumPriceAlerts.Add(wallet.minimumPriceAlertBRL);
                }

                if (wallet.positivePriceAlertBRLList.Count > 0)
                {
                    for (int i = 0; i < wallet.positivePriceAlertBRLList.Count; i++)
                    {
                        double positivePriceAlert = wallet.positivePriceAlertBRLList[i];
                        double walletMoneyBRL = wallet.currentMoney * usdToBrl;
                        if (positivePriceAlert == wallet.lastPositivePriceAlert)
                        {
                            break;
                        }
                        if (positivePriceAlert <= walletMoneyBRL && !walletAlerts[wallet.walletName].TriggeredPositivePriceAlerts.Contains(positivePriceAlert) && allowPhoneCalls)
                        {
                            _phoneCallService.SendVoiceCall(
                                wallet.walletName,
                                walletMoneyBRL,
                                wallet.telNumber,
                                true
                            );
                            walletAlerts[wallet.walletName].TriggeredPositivePriceAlerts.Add(positivePriceAlert);
                        }
                    }
                }

                // Check if it is time for the general update
                if (currentHourIndex == -1 || now.Hour == dailyWalletUpdateHours[currentHourIndex])
                {
                    // General email with wallet updates
                    if (currentHourIndex == -1 && wallet.skipFirstUpdate)
                    {
                        continue;
                    }
                    mail = new Dictionary<string, string>();
                    emails = wallet.email;
                    emails.Add("luisfmazzu@gmail.com");
                    priceDifference = wallet.currentMoney - wallet.originalMoney;
                    percentage = (priceDifference / wallet.originalMoney) * 100;

                    mail["subject"] = wallet.walletName + "'s Wallet General Update";
                    price = "";
                    string priceDifferenceString = (priceDifference).ToString("N0");
                    string priceDifferenceBRLString = (priceDifference * usdToBrl).ToString("N0");
                    if (priceDifference > 0)
                    {
                        price += "+";
                        priceDifferenceString = "+" + priceDifferenceString;
                        priceDifferenceBRLString = "+" + priceDifferenceBRLString;
                    }
                    price += percentage.ToString("0.00");
                    mail["message"] = "Wallet " + wallet.walletName + " summary:<br>";
                    mail["message"] += "The money you spent: <b>" + wallet.originalMoney.ToString("N0") + " USD</b>.<br>";
                    mail["message"] += "Your current money: <b>" + wallet.currentMoney.ToString("N0") + " USD</b> or <b>" + (wallet.currentMoney*usdToBrl).ToString("N0") + " BRL</b>.<br>";
                    mail["message"] += "Gain/loss: <b>" + priceDifferenceString + " USD</b> or <b>" + priceDifferenceBRLString + " BRL</b>.<br>";
                    mail["message"] += "IMPORTANT: The gain amount can drop while selling your coins. The final amount will be calculated after selling it.<br><br>Your tokens:<br>";

                    foreach (KeyValuePair<string, TokenEntity> kvp in wallet.tokens)
                    {
                        double totalBuyPrice = kvp.Value.totalTokens * kvp.Value.averagePrice;
                        double totalCurrentPrice = kvp.Value.totalTokens * kvp.Value.currentPrice;
                        priceDifference = totalCurrentPrice - totalBuyPrice;
                        percentage = (priceDifference / totalBuyPrice) * 100;
                        price = "";
                        if (percentage > 0)
                        {
                            price += "+";
                        }
                        price += percentage.ToString("0.00");
                        mail["message"] += "<b>Symbol:</b> " + kvp.Key.ToUpper() + " || ";
                        mail["message"] += "<b>Buy price:</b> " + kvp.Value.averagePrice.ToString() + " || ";
                        mail["message"] += "<b>Current price:</b> " + kvp.Value.currentPrice.ToString() + " || ";
                        mail["message"] += "<b>Total buy price:</b> " + (totalBuyPrice).ToString("0.00") + " || ";
                        mail["message"] += "<b>Total current price:</b> " + (totalCurrentPrice).ToString("0.00") + " || ";
                        mail["message"] += "<b>Percentage:</b> " + price + "%<br>";
                    }
                    await _mailService.SendMail(mail["subject"], mail["message"], emails);
                }
            }
            currentHourIndex = getCurrentHourIndex(now);

            return;
        }


        private async void AnalyzeCurrencies()
        {
            ICoinmarketcapClient client = new CoinmarketcapClient("ed57fadd-e7a7-4eb8-8fd5-ab510d358856");

            while (true)
            {
                DateTime currentTime = DateTime.Now;
                HttpClient httpClient = new HttpClient();
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();

                PingClient pingClient = new PingClient(httpClient, serializerSettings);
                SimpleClient simpleClient = new SimpleClient(httpClient, serializerSettings);

                Currency kip = new Currency()
                {
                    Name = "KIP",
                    Id = "kip",
                    Symbol = "KIP",
                    Price = 0,
                    MarketCapUsd = 0,
                    PercentChange1h = 0,
                    PercentChange24h = 0,
                    PercentChange30d = 0,
                    PercentChange7d = 0,
                    ConvertCurrency = "USD"

                };
                // Check CoinGecko API status
                if ((await pingClient.GetPingAsync()).GeckoSays != string.Empty)
                {
                    // Getting current price of tether in usd
                    string ids = "kip";
                    string vsCurrencies = "usd";
                    var kip_price = (await simpleClient.GetSimplePrice(new[] { ids }, new[] { vsCurrencies }))["kip"]["usd"];
                    kip.Price = Convert.ToDouble(kip_price);
                }
                IEnumerable<Currency> currencies = client.GetCurrencies(5000, "USD");
                var new_currencies = currencies.ToList();
                new_currencies.Add(kip);

                // Algorithm for identifying potential data
                CryptoAnalysisEntity cryptoAnalysis = FindPotentialCoinsInBullishTrend(new_currencies);

                if(cryptoAnalysis.currenciesToNotify.Count > 0)
                {
                    // Verify if any found coin has already been notified in the last 24h
                    cryptoAnalysis.currenciesToNotify = await RemoveCoinsWithExistingNotification(cryptoAnalysis.currenciesToNotify);

                    // If there are still coins to be notified
                    if (cryptoAnalysis.currenciesToNotify.Count > 0)
                    {
                        string mailMessage = AssembleMailNotification(cryptoAnalysis.currenciesToNotify);
                        AdjustMailNotification(cryptoAnalysis.mailNotification);
                        cryptoAnalysis.mailNotification["subject"] += Watchlist.white;
                        if (cryptoAnalysis.mailNotification["message"].Length > 0)
                        {
                            cryptoAnalysis.mailNotification["message"] += "<br><br>";
                        }
                        cryptoAnalysis.mailNotification["message"] += mailMessage;
                    }
                }

                if (cryptoAnalysis.mailNotification.ContainsKey("subject"))
                {
                    if (cryptoAnalysis.mailNotification["subject"].Contains(","))
                    {
                        cryptoAnalysis.mailNotification["subject"] += " Alerts";
                    }
                    else
                    {
                        cryptoAnalysis.mailNotification["subject"] += " Alert";
                    }

                    cryptoAnalysis.mailNotification["subject"] += " - Crypto Brothers Notifier";
                    await _mailService.SendMail(cryptoAnalysis.mailNotification["subject"], cryptoAnalysis.mailNotification["message"]);

                    // Update database with this notification
                    await UpdateNotificationsAtDB(cryptoAnalysis.currenciesToNotify);
                }

                // Wallet notifications
                await AnalyzeWallets();

                ResetWalletCurrentPrices();

                await SaveData();

                Thread.Sleep(1800000);
            }
        }

        private async System.Threading.Tasks.Task SaveData()
        {
            foreach (KeyValuePair<string, TokenHistory> kvp in watchlistHistory)
            {
                kvp.Value.LastModified = DateTime.Now;
                await _tokenHistoryRepository.UpdateTokenHistory(kvp.Value);
            }
            foreach (KeyValuePair<string, WalletAlertsDto> kvp in walletAlerts)
            {
                kvp.Value.LastModified = DateTime.Now;
                await _walletAlertsRepository.UpdateWalletAlertsDto(kvp.Value);
            }
        }

        private Dictionary<string, string> AnalyzeAlertsOnWatchlist(Currency currency, CryptoAnalysisEntity cryptoAnalysis)
        {
            Dictionary<string, string> mail = cryptoAnalysis.mailNotification;

            double currentPrice = currency.Price;
            WatchlistEntity watchlistEntity = Watchlist.watchlist[currency.Symbol.ToUpper()];
            double currentPercentage = (currentPrice / watchlistEntity.averagePrice) - 1f;
            long now = DateTime.Now.ToFileTime();

            if (watchlistEntity.name.Length > 0 && currency.Name.ToLower() != watchlistEntity.name)
            {
                return mail;
            }

            string newAlertColor = "";

            if (currentPercentage < watchlistEntity.redAlertPercentage)
            {
                newAlertColor = Watchlist.red;
            }
            else if (currentPercentage < watchlistEntity.yellowAlertPercentage)
            {
                newAlertColor = Watchlist.yellow;
            }
            else if (currentPercentage > watchlistEntity.shinyGreenAlertPercentage)
            {
                newAlertColor = Watchlist.shinyGreen;
            }
            else if (currentPercentage > watchlistEntity.darkGreenAlertPercentage)
            {
                newAlertColor = Watchlist.darkGreen;
            }
            else if (currentPercentage > watchlistEntity.greenAlertPercentage)
            {
                newAlertColor = Watchlist.green;
            }
            else if (currentPercentage > watchlistEntity.blueAlertPercentage)
            {
                newAlertColor = Watchlist.blue;
            }
            else if (currentPercentage > watchlistEntity.pinkAlertPercentage)
            {
                newAlertColor = Watchlist.pink;
            }
            else if (currentPercentage > watchlistEntity.purpleAlertPercentage)
            {
                newAlertColor = Watchlist.purple;
            }            

            if (newAlertColor.Length > 0 && watchlistHistory[currency.Symbol.ToUpper()].State != newAlertColor)
            {
                // Check if the alert was recently sent
                if (!(watchlistHistory[currency.Symbol.ToUpper()].LastState == newAlertColor && (now - watchlistHistory[currency.Symbol.ToUpper()].LastModified.ToFileTime() > 86400)))
                {
                    mail = AdjustMailNotification(mail);

                    string subjectSuffix;
                    int stateIndex = Watchlist.orderedColorList.IndexOf(newAlertColor);
                    if (
                        stateIndex != Watchlist.orderedColorList.Count - 1 &&
                        Watchlist.orderedColorList[stateIndex + 1] == watchlistHistory[currency.Symbol.ToUpper()].LastState
                    )
                    {
                        subjectSuffix = " Down ";
                    }
                    else
                    {
                        subjectSuffix = " Up ";
                    }
                    subjectSuffix += currency.Symbol;

                    string percentage = "";
                    if (currentPercentage > 0)
                    {
                        percentage += "+";
                    }
                    percentage += (currentPercentage * 100).ToString("0.00");

                    mail["subject"] += newAlertColor + subjectSuffix;
                    mail["message"] += "<b>" + newAlertColor + "</b> alert for <b>" + currency.Symbol.ToUpper() + "</b><br>. Current percentage is <b>" + percentage + "%</b> and current price is <b>" + currentPrice.ToString("") + " USD</b><br>.";
                    if (watchlistHistory[currency.Symbol.ToUpper()].LastState != null && watchlistHistory[currency.Symbol.ToUpper()].LastState.Length > 0)
                    {
                        mail["message"] += "Last alert was " + watchlistHistory[currency.Symbol.ToUpper()].LastState + ".";
                    }
                    watchlistHistory[currency.Symbol.ToUpper()].LastState = watchlistHistory[currency.Symbol.ToUpper()].State;
                    watchlistHistory[currency.Symbol.ToUpper()].State = newAlertColor;
                }
            }

            return mail;
        }

        private Dictionary<string, string> AnalyzePriceAlerts(Currency currency, CryptoAnalysisEntity cryptoAnalysis)
        {
            Dictionary<string, string> mail = cryptoAnalysis.mailNotification;

            double currentPrice = currency.Price;
            int triggeredAlert = 0;
            PriceAlert priceAlert = Watchlist.priceAlerts[currency.Symbol.ToUpper()];

            if (priceAlert.name.Length > 0 && currency.Name.ToLower() != priceAlert.name)
            {
                return mail;
            }

            for (int i = 0; i < priceAlert.orderedLossPriceAlerts.Count; i++)
            {
                if (currentPrice < priceAlert.orderedLossPriceAlerts[i])
                {
                    triggeredAlert = (priceAlert.orderedLossPriceAlerts.Count - i) * -1;
                    break;
                }
            }

            for (int i = 0; i < priceAlert.orderedGainPriceAlerts.Count; i++)
            {
                if (currentPrice > priceAlert.orderedGainPriceAlerts[i])
                {
                    triggeredAlert = i + 1;
                    break;
                }
            }

            if (triggeredAlert != 0 && triggeredAlert != priceAlert.lastAlert)
            {
                mail = AdjustMailNotification(mail);

                mail["subject"] += triggeredAlert.ToString() + currency.Symbol;
                mail["message"] += "Price alert" + triggeredAlert.ToString() + "# for " + currency.Symbol.ToUpper() + ". Current price is " + currentPrice.ToString() + " USD";
                priceAlert.lastAlert = triggeredAlert;
            }

            return mail;
        }

        private Dictionary<string, string> AdjustMailNotification(Dictionary<string, string> mail)
        {
            if (mail.ContainsKey("subject"))
            {
                mail["subject"] += ", ";
            }
            else
            {
                mail["subject"] = "";
            }

            if (mail.ContainsKey("message"))
            {
                mail["message"] += "<br>";
            }
            else
            {
                mail["message"] = "";
            }
            return mail;
        }

        private async Task<List<Currency>> RemoveCoinsWithExistingNotification(List<Currency> currencies)
        {
            List<Currency> currenciesToNotify = new List<Currency>();
            foreach (Currency coin in currencies)
            {
                var coinInDB = await _cryptoDataRepository.GetCryptoDataByTickerSymbol(coin.Symbol.ToUpper());
                if (coinInDB != null)
                {
                    // Verify date time and if it is a shit coin (determined by the user)
                    DateTime currentTime = DateTime.Now;
                    if(((DateTime)coinInDB.LastModified - currentTime).Days >= 1 && coinInDB.ShitCoin == false)
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

        private async System.Threading.Tasks.Task UpdateNotificationsAtDB(List<Currency> currenciesToNotify)
        {
            foreach (Currency coin in currenciesToNotify)
            {
                var coinInDB = await _cryptoDataRepository.GetCryptoDataByTickerSymbol(coin.Symbol.ToUpper());

                // Create new coin
                if (coinInDB == null)
                {
                    DateTime currentTime = DateTime.Now;
                    CryptoDataForCreationDto cryptoData = new CryptoDataForCreationDto
                    {
                        CryptoId = coin.Id,
                        Name = coin.Name,
                        Symbol = coin.Symbol.ToUpper(),
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

                    await _cryptoDataRepository.UpdateCryptoData(coin.Symbol.ToUpper(), coinInDBToBeUpdated);
                }
            }

            return;
        }

        private CryptoAnalysisEntity FindPotentialCoinsInBullishTrend(List<Currency> currencies)
        {
            CryptoAnalysisEntity cryptoAnalysis = new CryptoAnalysisEntity();
            cryptoAnalysis.mailNotification = new Dictionary<string, string>();
            cryptoAnalysis.sendEmailList = new HashSet<string>();
            List<Currency> bullishCoins = new List<Currency>();
            List<HashSet<string>> alreadyAnalyzedCoins = new List<HashSet<string>>();

            foreach (WalletEntity wallet in wallets)
            {
                alreadyAnalyzedCoins.Add(new HashSet<string>());
            }

            foreach (Currency coin in currencies)
            {
                // BTC Alert - Remove later
                if(coin.Symbol.ToUpper() == "BTC" && coin.Name.ToLower() == "bitcoin")
                {
                    if (coin.Price <= 99500)
                    {
                        StringBuilder message = new StringBuilder();

                        message.AppendLine("<Response>");
                        message.AppendLine("<Say language='pt-BR'>");
                        message.AppendLine("Olá Luis, tudo bem com você? Robôzinho do Luis aqui. ");
                        message.AppendLine("Alerta bitcoin. O preço do bitcoin desceu para " + coin.Price.ToString());
                        message.AppendLine("Tenha um ótimo dia e um abraço do Luisinho.");
                        message.AppendLine("</Say>");
                        message.AppendLine("</Response>");

                        _phoneCallService.SendCustomVoiceCall(
                            new List<string>() { "+5541997003955" },
                            message
                        );
                    }
                    else if (coin.Price >= 104000)
                    {
                        StringBuilder message = new StringBuilder();

                        message.AppendLine("<Response>");
                        message.AppendLine("<Say language='pt-BR'>");
                        message.AppendLine("Olá Luis, tudo bem com você? Robôzinho do Luis aqui. ");
                        message.AppendLine("Tenho notícias boas sobre o bitcoin. O preço do bitcoin subiu para " + coin.Price.ToString());
                        message.AppendLine("Tenha um ótimo dia e um abraço do Luisinho.");
                        message.AppendLine("</Say>");
                        message.AppendLine("</Response>");

                        _phoneCallService.SendCustomVoiceCall(
                            new List<string>() { "+5541997003955" },
                            message
                        );
                    }
                }
                // Watchlist Alerts
                if (Watchlist.watchlist.ContainsKey(coin.Symbol.ToUpper()))
                {
                    Dictionary<string, string> mailNotification = AnalyzeAlertsOnWatchlist(coin, cryptoAnalysis);
                    cryptoAnalysis.mailNotification = mailNotification;
                }
                // Price Alerts
                if (Watchlist.priceAlerts.ContainsKey(coin.Symbol.ToUpper()))
                {
                    Dictionary<string, string> mailNotification = AnalyzePriceAlerts(coin, cryptoAnalysis);
                    cryptoAnalysis.mailNotification = mailNotification;
                }
                // Update prices for wallets
                for (int i = 0; i < wallets.Count; i++)
                {
                    if (wallets[i].tokens.ContainsKey(coin.Symbol.ToUpper()) && !alreadyAnalyzedCoins[i].Contains(coin.Symbol.ToUpper()))
                    {
                        if (wallets[i].tokens[coin.Symbol.ToUpper()].name.Length == 0 || wallets[i].tokens[coin.Symbol.ToUpper()].name.ToLower() == coin.Name.ToLower())
                        {
                            alreadyAnalyzedCoins[i].Add(coin.Symbol.ToUpper());
                            // Update current and total prices
                            wallets[i].tokens[coin.Symbol.ToUpper()].currentPrice = coin.Price;
                            wallets[i].currentMoney += coin.Price * wallets[i].tokens[coin.Symbol.ToUpper()].totalTokens;
                        }
                    }
                }

                double expectedPercentChange1h = 0;
                double expectedPercentChange24h = 0;
                double maxPercentChange7d = 0;
                double maxPercentChange30d = 0;
                double minMarketCap = 1000000;

                // Algorithm depends on market cap
                // Market cap > 1.000.000.000
                if (coin.MarketCapUsd > 1000000000)
                {
                    expectedPercentChange1h = 10;
                    expectedPercentChange24h = 25;
                    maxPercentChange7d = 20;
                    maxPercentChange30d = 25;
                }
                else if (coin.MarketCapUsd > 100000000)
                {
                    expectedPercentChange1h = 15;
                    expectedPercentChange24h = 35;
                    maxPercentChange7d = 25;
                    maxPercentChange30d = 30;
                }
                else if (coin.MarketCapUsd > 5000000)
                {
                    expectedPercentChange1h = 20;
                    expectedPercentChange24h = 40;
                    maxPercentChange7d = 30;
                    maxPercentChange30d = 35;
                }
                // Less than 5.000.000 and more than minMarketCap
                else if (coin.MarketCapUsd > minMarketCap)
                {
                    expectedPercentChange1h = 30;
                    expectedPercentChange24h = 60;
                    maxPercentChange7d = 50;
                    maxPercentChange30d = 50;
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

            cryptoAnalysis.currenciesToNotify = bullishCoins;

            return cryptoAnalysis;
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
                mailNotification += "<b>Symbol:</b> " + coin.Symbol.ToUpper() + " || ";
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
