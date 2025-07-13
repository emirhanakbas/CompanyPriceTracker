using CompanyPriceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Domain.Repositories {
    public interface IUserRepository {
        Task AddSync(User user);
        Task<User?> GetByUsernameAsync(string username); // Kullanici adina gore kullaniciyi bulur
        Task<IEnumerable<User>> GetAllAsync();           // Tum kullanicilari getirir
    }
}
