using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Common.Domains;

namespace MongoPersistence.Domains
{
    class MongoCryptoTradingSimulationDomain : ICryptoTradingSimulationDomain
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
    }
}
