using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Domain.Entities;

namespace CompanyPriceTracker.Domain.Repositories {
    // Company Repository'deki fonksiyonların prototipleri, interface'leri. Core/Domain/Repository içinde interface tanımlayıp dışarıdan
    // implementasyon yapmak DEPENDECY INJECTION örneğidir. D.INJ., SOLID'deki DEPENDENCY INVERSION'un nasıl uygulandığıdır.
    public interface ICompanyRepository {
        Task AddAsync(Company company);            // yeni bir firma ekleme
        Task<IEnumerable<Company>> GetAllAsync(); // firmaların listesini alma
        Task<Company?> GetByIdAsync(string id);   // firma id'sine göre firma çekme 
    }
}
