using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace CompanyPriceTracker.Domain.Entities {
    public class Company {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }   // firma benzersiz ID
        [BsonElement("name")]
        public string Name { get; set; } // firma adı
        public Company() { }             // default constructor
        public Company(string name) { // dışarıdan alımda constructor overloading
            Name = name;
        }  
    }
} 