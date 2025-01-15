using AutoMapper;
using Common.Domains;
using Common.Repositories;
using MongoDB.Driver;
using MongoPersistence.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MongoPersistence
{
    public class WalletAlertsRepository : IWalletAlertsRepository
    {
        private readonly IMongoCollection<MongoWalletAlertsDto> _mongoCollection;
        public static IMapper Mapper { get; internal set; }

        public WalletAlertsRepository(MongoDbManager mongoDbManager)
        {
            _mongoCollection = mongoDbManager.GetCollection<MongoWalletAlertsDto>("WalletAlerts");

            var config = new MapperConfiguration(configuration =>
            {
                configuration.AllowNullCollections = true;
                configuration.CreateMap<WalletAlertsDto, MongoWalletAlertsDto>();
                configuration.CreateMap<List<WalletAlertsDto>, List<MongoWalletAlertsDto>>();
            });

            Mapper = config.CreateMapper();
        }

        public async Task<WalletAlertsDto> GetWalletAlertsDtoByName(string name)
        {
            var walletAlerts = await _mongoCollection.Find<MongoWalletAlertsDto>(u => u.Name.Equals(name)).FirstOrDefaultAsync();

            return walletAlerts;
        }

        public async Task<List<WalletAlertsDto>> GetAllWalletAlertsDto()
        {
            var walletAlerts = await _mongoCollection.Find<MongoWalletAlertsDto>(_ => true).ToListAsync();
            List<WalletAlertsDto> result = new List<WalletAlertsDto>();
            foreach (var walletAlert in walletAlerts)
            {
                result.Add(walletAlert);
            }

            return result;
        }

        public async Task InsertWalletAlertsDto(WalletAlertsDto walletAlertsDomain)
        {
            var mongoWalletAlertsDtoDomain = Mapper.Map<MongoWalletAlertsDto>(walletAlertsDomain);
            await _mongoCollection.InsertOneAsync(mongoWalletAlertsDtoDomain);
        }

        public async Task UpdateWalletAlertsDto(WalletAlertsDto walletAlertsDomain)
        {
            var mongoWalletAlertsDtoDomain = Mapper.Map<MongoWalletAlertsDto>(walletAlertsDomain);
            mongoWalletAlertsDtoDomain.LastModified = DateTime.UtcNow;

            var filter = Builders<MongoWalletAlertsDto>.Filter.Eq(x => x.Name, walletAlertsDomain.Name);

            var updateResult = await _mongoCollection.ReplaceOneAsync(filter, mongoWalletAlertsDtoDomain);
        }

        public async Task DeleteWalletAlertsDto(string walletAlertsTicker)
        {

        }
    }
}
