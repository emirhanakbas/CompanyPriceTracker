using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Domain.Entities;

namespace CompanyPriceTracker.Domain.Repositories {
    // Price Repository'deki fonksiyonların prototipleri, interface'leri. Core/Domain/Repository içinde interface tanımlayıp dışarıdan
    // implementasyon yapmak DEPENDECY INJECTION örneğidir. DI, SOLID'deki DEPENDENCY INVERSION'un nasıl uygulandığıdır.
    public interface ICompanyPriceRepository {
        Task AddAsync(CompanyPrice companyprice);
        Task<CompanyPrice?> GetPriceByCompanyAndYearAsync(string companyId, int year);
        Task<IEnumerable<CompanyPrice>> GetPricesByCompanyIdAsync(string companyId);
        Task<CompanyPrice?> GetLatestPriceByCompanyIdAsync(string companyId);
    }
}
