using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Common.Domains;

namespace MongoPersistence.Domains
{
    public class MongoCryptoDataDomain : ICryptoDataDomain
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
    }
}
