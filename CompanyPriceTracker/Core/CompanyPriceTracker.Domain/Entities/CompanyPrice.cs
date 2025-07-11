using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Domain.Entities {
    public class CompanyPrice {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }                                    // firmaların her yıla göre fiyatının benzersiz ID'si  
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("companyId")]
        public string CompanyId { get; set; }                             // company'nin kendisinden gelen benzersiz ID  
        [BsonElement("year")]
        public int Year { get; set; }                                     // hangi yılın fiyatı 
        [BsonElement("price")]
        public double Price { get; set; }                                 // firma uzman/gün fiyatı  
        public CompanyPrice() { }                                         // default constructor
        public CompanyPrice(string CompanyId, int year, double price) {   // dışarıdan firma kaydedilirken constructor overloading
            this.CompanyId = CompanyId;
            Year = year;
            Price = price;
        } 
    }
}
