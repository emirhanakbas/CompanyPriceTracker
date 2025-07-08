using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Domain.Repositories;  // ICompanyRepository.cs
using CompanyPriceTracker.Domain.Entities;      // Company.cs
using CompanyPriceTracker.Persistence.Settings; // MongoDbSettings
using Microsoft.Extensions.Options;
using MongoDB.Driver;                           // MongoDB C# temel sürücüleri

namespace CompanyPriceTracker.Persistence.Repositories {
    public class CompanyRepository : ICompanyRepository {
        private readonly IMongoDatabase _database;                       // mongodb veritabanı referansı                   
        private readonly IMongoCollection<Company> _companiesCollection; // 'Company' tipindeki verilerin tutulduğu mongodb referansı

        public CompanyRepository(IOptions<MongoDbSettings> settings) {
            var client = new MongoClient(settings.Value.ConnectionString);        // mongodb sunucusuna genel bağlantıyı kuran client
            _database = client.GetDatabase(settings.Value.DatabaseName);          // settings dosyasında oluşturduğumuz db name'in referansını _database'e veririz
            _companiesCollection = _database.GetCollection<Company>("companies"); // 
        }

        public async Task AddAsync(Company entity) { // yeni bir firma ekleme
            await _companiesCollection.InsertOneAsync(entity); // InsertOneAsync firma listesine tek bir firma ekler
                                                               // 'entity' Id property'sine [BsonId] attribute eklendiği için mongodb bu belgeyi kaydederken otomatik benzersiz bir id verecektir
        }

        public async Task<IEnumerable<Company>> GetAllAsync() { // tüm firmaları listeleme
            return await _companiesCollection.Find(_ => true).ToListAsync(); // Find(_ => true) ifadesi 'companies' koleksiyonundaki tüm firmaları getirir ve ToListAsync() ile asenkron listeler
        }

        public async Task<Company?> GetByIdAsync(string id) { // belirli bir firmayı id'sine göre getirme  
            return await _companiesCollection.Find(company => company.Id == id).FirstOrDefaultAsync();
        }
    }
}
