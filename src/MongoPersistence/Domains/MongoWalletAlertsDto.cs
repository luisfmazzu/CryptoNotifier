using Common.Domains;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoPersistence.Domains
{
    public class MongoWalletAlertsDto : WalletAlertsDto
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
    }
}
