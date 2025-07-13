using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Domain.Entities {
    public class User {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        [BsonElement("username")]
        public string Username { get; set; } = null!;
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;
        [BsonElement("roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
