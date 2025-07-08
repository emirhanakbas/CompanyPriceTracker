using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Domain.Repositories;  // ICompanyPriceRepository.cs
using CompanyPriceTracker.Domain.Entities;      // CompanyPrice.cs
using CompanyPriceTracker.Persistence.Settings; // MongoDbSettings
using Microsoft.Extensions.Options;
using MongoDB.Driver;                           // MongoDB C# temel sürücüleri

namespace CompanyPriceTracker.Persistence.Repositories {
    public class CompanyPriceRepository : ICompanyPriceRepository {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<CompanyPrice> _companyPricesCollection;

        public CompanyPriceRepository(IOptions<MongoDbSettings> settings) {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            _companyPricesCollection = _database.GetCollection<CompanyPrice>("companyPrices");
        }

        public async Task AddAsync(CompanyPrice entity) { // firma fiyatı ekleme 
            await _companyPricesCollection.InsertOneAsync(entity);
        }

        public async Task<CompanyPrice?> GetPriceByCompanyAndYearAsync(string companyId, int year) { // birden fazla parametreye göre arama yapmak için filtre oluşturma
            return await _companyPricesCollection.Find(companyPrice => companyPrice.CompanyId == companyId && companyPrice.Year == year).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CompanyPrice>> GetPricesByCompanyIdAsync(string companyId) { // belirli firmanın id'sine göre fiyat getirme
            return await _companyPricesCollection.Find(companyPrice => companyPrice.CompanyId == companyId).ToListAsync();
        }

        public async Task<CompanyPrice?> GetLatestPriceByCompanyIdAsync(string companyId) {
            return await _companyPricesCollection.Find(cp => cp.CompanyId == companyId).SortByDescending(cp => cp.Year).FirstOrDefaultAsync();
        }
    }
}
