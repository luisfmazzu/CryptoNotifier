using System;
using System.Collections.Generic;
using System.Text;

namespace MongoPersistence
{
    using MongoDB.Driver;
    using System;
    using System.Threading.Tasks;
    using Common.Domains;
    using Common.Repositories;
    using AutoMapper;
    using System.Collections.Generic;
    using global::MongoPersistence.Domains;

    namespace MongoPersistence
    {
        public class TokenHistoryRepository : ITokenHistoryRepository
        {
            private readonly IMongoCollection<MongoTokenHistory> _mongoCollection;
            public static IMapper Mapper { get; internal set; }

            public TokenHistoryRepository(MongoDbManager mongoDbManager)
            {
                _mongoCollection = mongoDbManager.GetCollection<MongoTokenHistory>("TokenHistory");

                var config = new MapperConfiguration(configuration =>
                {
                    configuration.AllowNullCollections = true;
                    configuration.CreateMap<TokenHistory, MongoTokenHistory>();
                    configuration.CreateMap<List<TokenHistory>, List<MongoTokenHistory>>();
                });

                Mapper = config.CreateMapper();
            }

            public async Task<List<TokenHistory>> GetAllTokenHistoryDto()
        {
            var tokenHistoryList = await _mongoCollection.Find<MongoTokenHistory>(_ => true).ToListAsync();
            List<TokenHistory> result = new List<TokenHistory>();
            foreach (var tokenHistory in tokenHistoryList)
            {
                result.Add(tokenHistory);
            }

            return result;
        }

            public async Task<TokenHistory> GetTokenHistoryBySymbol(string symbol)
            {
                var tokenHistory = await _mongoCollection.Find<MongoTokenHistory>(u => u.Symbol.Equals(symbol)).FirstOrDefaultAsync();

                return tokenHistory;
            }

            public async Task InsertTokenHistory(TokenHistory tokenHistoryDomain)
            {
                var mongoTokenHistoryDomain = Mapper.Map<MongoTokenHistory>(tokenHistoryDomain);
                await _mongoCollection.InsertOneAsync(mongoTokenHistoryDomain);
            }

            public async Task UpdateTokenHistory(TokenHistory tokenHistoryDomain)
            {
                var mongoTokenHistoryDomain = Mapper.Map<MongoTokenHistory>(tokenHistoryDomain);
                mongoTokenHistoryDomain.LastModified = DateTime.UtcNow;

                var filter = Builders<MongoTokenHistory>.Filter.Eq(x => x.Symbol, tokenHistoryDomain.Symbol);

                var updateResult = await _mongoCollection.ReplaceOneAsync(filter, mongoTokenHistoryDomain);
            }

            public async Task DeleteTokenHistory(string tokenHistoryTicker)
            {

            }
        }
    }

}
