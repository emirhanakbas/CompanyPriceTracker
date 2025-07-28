using CompanyPriceTracker.Domain.Entities;
using CompanyPriceTracker.Domain.Repositories;
using CompanyPriceTracker.Persistence.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Persistence.Repositories {
    public class UserRepository : IUserRepository {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IOptions<MongoDbSettings> settings) {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _usersCollection = database.GetCollection<User>("users");       // users adında bir koleksiyon
            Console.WriteLine("MongoDB Connection String:");
            Console.WriteLine(settings.Value.ConnectionString);
        }

        public async Task AddSync(User user) {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<User?> GetByUsernameAsync(string username) {
            return await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync() {
            return await _usersCollection.Find(_ => true).ToListAsync();
        }
    }
}
