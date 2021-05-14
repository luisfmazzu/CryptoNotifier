using MongoPersistence.Domains;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Common.Domains;
using Common.Repositories;
using AutoMapper;
using System.Collections.Generic;

namespace MongoPersistence
{
    public class CryptoDataRepository : ICryptoDataRepository
    {
        private readonly IMongoCollection<MongoCryptoDataDomain> _mongoCollection;
        public static IMapper Mapper { get; internal set; }

        public CryptoDataRepository(MongoDbManager mongoDbManager)
        {
            _mongoCollection = mongoDbManager.GetCollection<MongoCryptoDataDomain>("CryptoDataDomain");

            var config = new MapperConfiguration(configuration =>
            {
                configuration.AllowNullCollections = true;
                configuration.CreateMap<ICryptoDataDomain, MongoCryptoDataDomain>();
                configuration.CreateMap<List<ICryptoDataDomain>, List<MongoCryptoDataDomain>>();
            });

            Mapper = config.CreateMapper();
        }

        public async Task<ICryptoDataDomain> GetCryptoDataById(string cryptoDataId)
        {
            var cryptoData = await _mongoCollection.Find<MongoCryptoDataDomain>(u => u.CryptoId.Equals(cryptoDataId)).FirstOrDefaultAsync();

            return cryptoData;
        }

        public async Task<ICryptoDataDomain> GetCryptoDataByTickerSymbol(string tickerSymbol)
        {
            string lowercaseTickerSymbol = tickerSymbol.ToLower();
            var cryptoData = await _mongoCollection.Find<MongoCryptoDataDomain>(u => u.Symbol.Equals(lowercaseTickerSymbol)).FirstOrDefaultAsync();

            return cryptoData;
        }

        public async Task InsertCryptoData(ICryptoDataDomain cryptoDataDomain)
        {
            var mongoCryptoDataDomain = Mapper.Map<MongoCryptoDataDomain>(cryptoDataDomain);
            await _mongoCollection.InsertOneAsync(mongoCryptoDataDomain);
        }

        public async Task UpdateCryptoData(string tickerSymbol, ICryptoDataDomain cryptoDataDomain)
        {
            var mongoCryptoDataDomain = (MongoCryptoDataDomain)cryptoDataDomain;
            mongoCryptoDataDomain.LastModified = DateTime.UtcNow;

            var updatedCryptoData = await _mongoCollection.ReplaceOneAsync<MongoCryptoDataDomain>(u => u.Symbol.Equals(tickerSymbol), mongoCryptoDataDomain, new ReplaceOptions { IsUpsert = true });
        }

        public async Task DeleteCryptoData(string cryptoDataTicker)
        {

        }

        public async Task<List<ICryptoDataDomain>> GetAllCryptoDatas()
        {
            List<MongoCryptoDataDomain> cryptoDatas = await _mongoCollection.Find<MongoCryptoDataDomain>(_ => true).ToListAsync();
            List<ICryptoDataDomain> cryptoDatasList = new List<ICryptoDataDomain>();
            foreach(MongoCryptoDataDomain cryptoData in cryptoDatas)
            {
                cryptoDatasList.Add(cryptoData);
            }
            return cryptoDatasList;
        }
    }
}
