using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CryptoNotifier.Services
{
    public class WatchlistEntity
    {
        public string name = "";
        public double averagePrice;
        public double redAlertPercentage = -0.3f;
        public double yellowAlertPercentage = -0.2f;
        public double purpleAlertPercentage = 0.5f;
        public double pinkAlertPercentage = 1f;
        public double blueAlertPercentage = 1.5f;
        public double greenAlertPercentage = 2f;
        public double darkGreenAlertPercentage = 2.5f;
        public double shinyGreenAlertPercentage = 3f;
    }

    public class PriceAlert
    {
        public List<double> orderedLossPriceAlerts;
        public List<double> orderedGainPriceAlerts;
        public string name = "";
        public int lastAlert = 0;
    }

    public class Watchlist
    {
        public static string black = "Black";
        public static string white = "White";
        public static string red = "Red";
        public static string yellow = "Yellow";
        public static string purple = "Purple";
        public static string pink = "Pink";
        public static string blue = "Blue";
        public static string green = "Green";
        public static string darkGreen = "DarkGreen";
        public static string shinyGreen = "ShinyGreen";
        public static List<string> orderedColorList = new List<string>()
        {
            red, yellow, purple, pink, blue, green, darkGreen, shinyGreen
        };

        public static Dictionary<string, WatchlistEntity> watchlist = new Dictionary<string, WatchlistEntity>()
        {
            { "CGPT", new WatchlistEntity()
                {
                    averagePrice = 0.13097f,
                }
            },
            { "OFN", new WatchlistEntity()
                {
                    averagePrice = 0.1f,
                }
            },
            { "FORT", new WatchlistEntity()
                {
                    averagePrice = 0.0992f,
                }
            },
            { "AXGT", new WatchlistEntity()
                {
                    averagePrice = 0.29f,
                    name = "axondao governance token"
                }
            },
            { "VRA", new WatchlistEntity()
                {
                    averagePrice = 0.00337f,
                }
            },
            { "PAAL", new WatchlistEntity()
                {
                    averagePrice = 0.1409f,
                }
            },
            { "0X0", new WatchlistEntity()
                {
                    averagePrice = 0.13257f,
                }
            },
            { "GPU", new WatchlistEntity()
                {
                    averagePrice = 0.7698f,
                }
            },
            { "DSYNC", new WatchlistEntity()
                {
                    averagePrice = 0.27701f,
                }
            },
            { "NEAR", new WatchlistEntity()
                {
                    averagePrice = 5.8f,
                }
            },
            { "PYR", new WatchlistEntity()
                {
                    averagePrice = 2.79f
                }
            },
            { "MYRIA", new WatchlistEntity()
                {
                    averagePrice = 0.00206f,
                }
            },
            { "BERRY", new WatchlistEntity()
                {
                    averagePrice = 0.2717f,
                    name = "strawberry ai"
                }
            },
            { "AIT", new WatchlistEntity()
                {
                    averagePrice = 0.10373f,
                    name = "ait protocol",
                }
            },
            { "PHA", new WatchlistEntity()
                {
                    averagePrice = 0.1339f,
                }
            },
            { "AITECH", new WatchlistEntity()
                {
                    averagePrice = 0.08448f,
                }
            },
            { "SDAO", new WatchlistEntity()
                {
                    averagePrice = 0.2185f,
                }
            },
            { "NUM", new WatchlistEntity()
                {
                    averagePrice = 0.05891f,
                }
            },
            { "VAI", new WatchlistEntity()
                {
                    averagePrice = 0.08475f,
                    name = "vaiot",
                }
            },
            { "DCK", new WatchlistEntity()
                {
                    averagePrice = 0.02509f
                }
            },
            { "BLENDR", new WatchlistEntity()
                {
                    averagePrice = 0.44f
                }
            },
            { "GTAI", new WatchlistEntity()
                {
                    averagePrice = 0.5775f
                }
            },
            { "AIPAD", new WatchlistEntity()
                {
                    averagePrice = 0.05249f
                }
            },
            { "VR", new WatchlistEntity()
                {
                    averagePrice = 0.01118f
                }
            },
        };

        public static Dictionary<string, PriceAlert> priceAlerts = new Dictionary<string, PriceAlert>()
        {
            { "BTC", new PriceAlert()
                {
                    orderedGainPriceAlerts = new List<double>()
                    {
                        100000f, 105000f, 110000f
                    },
                    orderedLossPriceAlerts = new List<double>()
                    {
                        85000f, 88000f, 89999f, 92000f, 95000f
                    },
                    name = "bitcoin"
                }
            },
        };

        public static WalletEntity luisWallet = new WalletEntity()
        {
            walletName = "Luis",
            totalMoneySpent = 0,
            email = new HashSet<string>() { "luisfmazzu@gmail.com" },
            tokens = new Dictionary<string, TokenEntity>()
            {
                {"BLENDR", new TokenEntity()
                    {
                        name = "blendr network",
                        averagePrice = 0.44817,
                        totalTokens = 9930.29714,
                    }
                },
                {"NMT", new TokenEntity()
                    {
                        name = "netmind token",
                        averagePrice = 1.91578,
                        totalTokens = 2323.07387,
                    }
                },
                {"PAAL", new TokenEntity()
                    {
                        name = "paal ai",
                        averagePrice = 0.1409,
                        totalTokens = 76909.03119,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.73518,
                        totalTokens = 11449.35,
                    }
                },
                {"DCK", new TokenEntity()
                    {
                        name = "dexcheck ai",
                        averagePrice = 0.02509,
                        totalTokens = 153433.06483,
                    }
                },
                { "BCUT", new TokenEntity()
                    {
                        averagePrice = 0.0377200,
                        totalTokens = 195620.82440,
                    }
                },
                { "AIN", new TokenEntity()
                    {
                        averagePrice = 0.00822,
                        totalTokens = 370371.57000,
                        name = "ai network",
                    }
                },
            },
            skipFirstUpdate = true,
            originalMoney = 129310.3448275862,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 1190000,
            positivePriceAlertBRLList = new List<double>() { 
                5000000,
                4500000,
                4000000,
                3500000,
                3000000,
                2500000,
                2000000,
                1700000,
                1568000
            },
        };

        public static WalletEntity gusWallet1 = new WalletEntity()
        {
            walletName = "Gustavo",
            totalMoneySpent = 0,
            email = new HashSet<string>() { "" },
            tokens = new Dictionary<string, TokenEntity>()
            {
                {"BLENDR", new TokenEntity()
                    {
                        name = "blendr network",
                        averagePrice = 0.44817,
                        totalTokens = 3002.182857,
                    }
                },
                {"CHAPZ", new TokenEntity()
                    {
                        averagePrice = 0.0017,
                        totalTokens = 1003758.49,
                    }
                },
                {"NMT", new TokenEntity()
                    {
                        name = "netmind token",
                        averagePrice = 1.91578,
                        totalTokens = 1116.516126,
                    }
                },
                { "TADA", new TokenEntity()
                    {
                        averagePrice = ((124896.46*0.0295) + (102043*0.0345))/(124896.46 + 102043),
                        name = "ta-da",
                        totalTokens = 124896.46 + 102043,
                    }
                },
                {"PAAL", new TokenEntity()
                    {
                        name = "paal ai",
                        averagePrice = 0.18418,
                        totalTokens = 7117.96,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.6432,
                        totalTokens = 3420.397854,
                    }
                },
                {"DCK", new TokenEntity()
                    {
                        name = "dexcheck ai",
                        averagePrice = 0.0256,
                        totalTokens = 66406.2496,
                    }
                },
            },
            skipFirstUpdate = true,
            originalMoney = 17400,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 112000,
            positivePriceAlertBRLList = new List<double>() { 350000, 300000, 250000, 200000, 170000 },
        };

        public static WalletEntity modenaWallet = new WalletEntity()
        {
            walletName = "Modena",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                {"PYR", new TokenEntity()
                    {
                        averagePrice = 3.1278,
                        totalTokens = 54.25,
                    }
                },
                {"NEAR", new TokenEntity()
                    {
                        name = "near protocol",
                        averagePrice = 6.381,
                        totalTokens = 26.6589,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.6155,
                        totalTokens = 275.19,
                    }
                },
            },
            skipFirstUpdate = true,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 3150,
            positivePriceAlertBRLList = new List<double>() { 20000, 15000, 10000, 7000 },
        };

        public static WalletEntity henriqueWallet1 = new WalletEntity()
        {
            walletName = "Bianca",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "OORT", new TokenEntity()
                    {
                        name = "oort",
                        averagePrice = 0.099135,
                        totalTokens = 7463.49,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.813297,
                        totalTokens = 909.13,
                    }
                },
                { "TADA", new TokenEntity()
                    {
                        averagePrice = 0.0338,
                        name = "ta-da",
                        totalTokens = 330.4512/0.0338,
                    }
                },
            },
            skipFirstUpdate = true,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 12000,
            positivePriceAlertBRLList = new List<double>() { 40000, 35000, 30000, 25000, 20000 },
        };

        public static WalletEntity henriqueWallet2 = new WalletEntity()
        {
            walletName = "Henrique",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "TADA", new TokenEntity()
                    {
                        averagePrice = 0.0334,
                        name = "ta-da",
                        totalTokens = 412.9507/0.0334,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.9542,
                        totalTokens = 411.3/0.9542,
                    }
                },
            },
            skipFirstUpdate = true,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 5050,
            positivePriceAlertBRLList = new List<double>() { 15000, 10000, 7000 },
        };

        public static WalletEntity bibiWallet = new WalletEntity()
        {
            walletName = "Bibi",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "OORT", new TokenEntity()
                    {
                        name = "oort",
                        averagePrice = 0.10261,
                        totalTokens = 43851.6,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.78200,
                        totalTokens = 8534.68300,
                    }
                },
                {"PAAL", new TokenEntity()
                    {
                        averagePrice = 0.1409,
                        totalTokens = 19220,
                        name = "paal ai",
                    }
                },
                { "OFN", new TokenEntity()
                    {
                        averagePrice = 0.156510,
                        name = "openfabric ai",
                        totalTokens = 10435.89,
                    }
                },
                { "SDAO", new TokenEntity()
                    {
                        averagePrice = 0.252430,
                        totalTokens = 6435.53000,
                    }
                },
            },
            skipFirstUpdate = true,
            originalMoney = 17241.37931034483,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 125000,
            positivePriceAlertBRLList = new List<double>() { 400000, 350000, 300000, 250000, 200000 },
        };

        public static WalletEntity meriWallet = new WalletEntity()
        {
            walletName = "Meri",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "TADA", new TokenEntity()
                    {
                        averagePrice = 0.0296065,
                        name = "ta-da",
                        totalTokens = 111949,
                    }
                },
                { "NAVX", new TokenEntity()
                    {
                        averagePrice = 0.172840,
                        totalTokens = 19566,
                    }
                },
            },
            skipFirstUpdate = true,
            originalMoney = 6779.66,
            minimumPriceAlertBRL = 31000,
            telNumber = new List<string>() { "" },
        };

        public static WalletEntity alejandroWallet = new WalletEntity()
        {
            walletName = "Alejandro",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "TADA", new TokenEntity()
                    {
                        averagePrice = ((3634.12*0.0338) + (2436.87*0.0339)) / (3634.12 + 2436.87),
                        name = "ta-da",
                        totalTokens = 3634.12 + 2436.87,
                    }
                },
                {"GTAI", new TokenEntity()
                    {
                        name = "gt protocol",
                        averagePrice = 0.7538,
                        totalTokens = 95.3699/0.7538,
                    }
                },
                { "VIA", new TokenEntity()
                    {
                        averagePrice = ((0.0866*1120.97)+(507.66*0.1604))/(1120.97+507.66),
                        name = "octavia ai",
                        totalTokens = 1120.97+507.66,
                    }
                },
                { "AIN", new TokenEntity()
                    {
                        averagePrice = 0.00824,
                        totalTokens = 13031+7018.8,
                        name = "ai network",
                    }
                },
            },
            skipFirstUpdate = true,
            telNumber = new List<string>() { "" },
            minimumPriceAlertBRL = 3800,
            positivePriceAlertBRLList = new List<double>() { 15000, 10000, 7000 },
        };

        public static WalletEntity cesarWallet = new WalletEntity()
        {
            walletName = "Cesar",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "OFN", new TokenEntity()
                    {
                        averagePrice = 0.1565,
                        name = "openfabric ai",
                        totalTokens = 5217.94,
                    }
                },
                { "SDAO", new TokenEntity()
                    {
                        averagePrice = 0.252430,
                        totalTokens = 3217.76000,
                    }
                },
                { "AIT", new TokenEntity()
                    {
                        averagePrice = 0.11701,
                        totalTokens = 28178.66000,
                        name = "ait protocol"
                    }
                },
                { "XX", new TokenEntity()
                    {
                        averagePrice = 0.077360,
                        totalTokens = 29722.77,
                        name = "xx network"
                    }
                },

            },
            skipFirstUpdate = true,
            originalMoney = 9092.83,
            minimumPriceAlertBRL = 36000,
            telNumber = new List<string>() { "" },
        };

        public static WalletEntity keiaWallet = new WalletEntity()
        {
            walletName = "Keia",
            email = new HashSet<string>() { "" },
            totalMoneySpent = 0,
            tokens = new Dictionary<string, TokenEntity>()
            {
                { "BERRY", new TokenEntity()
                    {
                        averagePrice = 0.429000,
                        name = "strawberry ai",
                        totalTokens = 2674.05,
                    }
                },
                { "VIA", new TokenEntity()
                    {
                        averagePrice = 0.132760,
                        name = "octavia ai",
                        totalTokens = 13573.365472,
                    }
                },
                { "AIT", new TokenEntity()
                    {
                        averagePrice = 0.11800,
                        totalTokens = 15343.22034,
                        name = "ait protocol"
                    }
                },
                { "DEAI", new TokenEntity()
                    {
                        averagePrice = 0.6905,
                        totalTokens = 2012.05,
                        name = "zero1 labs"
                    }
                }
            },
            skipFirstUpdate = true,
            originalMoney = 6557.37,
            minimumPriceAlertBRL = 29000,
            telNumber = new List<string>() { "" },
        };

        public static Dictionary<string, FixedTokenEntity> fixedTokens = new Dictionary<string, FixedTokenEntity>()
        {
                { "TADA", new FixedTokenEntity()
                    {
                        sellPrice = 0.3,
                        highestPrice = 0.4899
                    }
                },
                { "BERRY", new FixedTokenEntity()
                    {
                        sellPrice = 1.000,
                        highestPrice = 1.5
                    }
                },
                { "AGX", new FixedTokenEntity()
                    {
                        sellPrice = 0.600,
                        highestPrice = 0.9981
                    }
                },
                {"GTAI", new FixedTokenEntity()
                    {
                        sellPrice = 3,
                        highestPrice = 0.3279
                    }
                },
                { "VIA", new FixedTokenEntity()
                    {
                        sellPrice = 1.2,
                        highestPrice = 2.85
                    }
                },
                { "AIN", new FixedTokenEntity()
                    {
                        sellPrice = 0.1,
                        highestPrice = 0.2648
                    }
                },
                { "PAAL", new FixedTokenEntity()
                    {
                        sellPrice = 0.500,
                        highestPrice = 0.8598
                    }
                },
                { "OORT", new FixedTokenEntity()
                    {
                        sellPrice = 0.8,
                        highestPrice = 1.18
                    }
                },
                {"PYR", new FixedTokenEntity()
                    {
                        sellPrice = 8,
                        highestPrice = 49.24
                    }
                },
                {"NEAR", new FixedTokenEntity()
                    {
                        sellPrice = 10,
                        highestPrice = 20.44
                    }
                },
                {"BLENDR", new FixedTokenEntity()
                    {
                        sellPrice = 2.24,
                        highestPrice = 4.31
                    }
                },
                {"NMT", new FixedTokenEntity()
                    {
                        sellPrice = 8,
                        highestPrice = 16.09
                    }
                },
                {"SDAO", new FixedTokenEntity()
                    {
                        sellPrice = 1,
                        highestPrice = 6.62
                    }
                },
                {"DCK", new FixedTokenEntity()
                    {
                        sellPrice = 0.1,
                        highestPrice = 0.1827
                    }
                },
                { "BCUT", new FixedTokenEntity()
                    {
                        sellPrice = 0.2,
                        highestPrice = 0.5359
                    }
                },
                { "VRA", new FixedTokenEntity()
                    {
                        sellPrice = 0.01,
                        highestPrice = 0.08621
                    }
                },
                { "0X0", new FixedTokenEntity()
                    {
                        sellPrice = 0.35,
                        highestPrice = 0.5004
                    }
                },
                { "GPU", new FixedTokenEntity()
                    {
                        sellPrice = 2,
                        highestPrice = 2.85
                    }
                },
                { "RING", new FixedTokenEntity()
                    {
                        sellPrice = 0.5,
                        highestPrice = 0.8704
                    }
                },
                { "MYRIA", new FixedTokenEntity()
                    {
                        sellPrice = 0.01,
                        highestPrice = 0.01668
                    }
                },
                { "GSWIFT", new FixedTokenEntity()
                    {
                        sellPrice = 0.5,
                        highestPrice = 0.81
                    }
                },
                { "AITECH", new FixedTokenEntity()
                    {
                        sellPrice = 0.25,
                        highestPrice = 0.4939
                    }
                },
                { "VAI", new FixedTokenEntity()
                    {
                        sellPrice = 0.25,
                        highestPrice = 3.60
                    }
                },
                { "EMC", new FixedTokenEntity()
                    {
                        sellPrice = 1.0,
                        highestPrice = 2.16
                    }
                },
                { "VXV", new FixedTokenEntity()
                    {
                        sellPrice = 1.500,
                        highestPrice = 19.19
                    }
                },
                { "AGRS", new FixedTokenEntity()
                    {
                        sellPrice = 8,
                        highestPrice = 38.86
                    }
                },
                { "AIT", new FixedTokenEntity()
                    {
                        sellPrice = 0.8,
                        highestPrice = 1.20
                    }
                },
                { "OFN", new FixedTokenEntity()
                    {
                        sellPrice = 0.6,
                        highestPrice = 0.91
                    }
                },
                { "CHAPZ", new FixedTokenEntity()
                    {
                        sellPrice = 0.005,
                        highestPrice = 0.009815
                    }
                },
                { "KIP", new FixedTokenEntity()
                    {
                        sellPrice = 0.1,
                        highestPrice = 0.04471
                    }
                },
                { "NAVX", new FixedTokenEntity()
                    {
                        sellPrice = 0.35,
                        highestPrice = 0.4171
                    }
                },
                { "TOSHI", new FixedTokenEntity()
                    {
                        sellPrice = 0.001,
                        highestPrice = 0.0007938
                    }
                },
                { "MASA", new FixedTokenEntity()
                    {
                        sellPrice = 0.4,
                        highestPrice = 0.8032
                    }
                },
                { "DOAI", new FixedTokenEntity()
                    {
                        sellPrice = 0.025,
                        highestPrice = 0.03595
                    }
                },
                { "DEAI", new FixedTokenEntity()
                    {
                        sellPrice = 1.4,
                        highestPrice = 1.21
                    }
                },
                { "XX", new FixedTokenEntity()
                    {
                        sellPrice = 0.24,
                        highestPrice = 0.7703
                    }
                },
        };
    }

    public class CryptoAnalysisEntity
    {
        public List<Currency> currenciesToNotify { get; set; }
        public Dictionary<string, string> mailNotification { get; set; }
        public HashSet<string> sendEmailList { get; set; }
    }

    public class WalletEntity
    {
        public Dictionary<string, TokenEntity> tokens { get; set; }
        public double totalMoneySpent { get; set; }
        public double currentMoney = 0;
        public double originalMoney;
        public double minimumPriceAlertBRL;
        public List<double> positivePriceAlertBRLList = new List<double>();
        public double lastPositivePriceAlert = 0;
        public string walletName;
        public HashSet<string> email;
        public bool skipFirstUpdate;
        public List<string> telNumber = new List<string>();
    }

    public class TokenEntity
    {
        public string name = "";
        public double averagePrice;
        public double currentPrice = 0;
        public double totalTokens;
        public bool sentPriceAlert = false;
    }

    public class FixedTokenEntity
    {
        public double sellPrice;
        public double highestPrice;
    }
}