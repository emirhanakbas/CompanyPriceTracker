using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.DTOs.Company;

namespace CompanyPriceTracker.Application.Abstractions.Services {
    public interface ICompanyService {
        Task<CompanyResponseWithDetailsDTO> CreateCompanyAsync(CompanyCreateWithDetailsDTO companyDTO);
        Task<IEnumerable<CompanyResponseWithDetailsDTO>> GetAllCompaniesAsync();
        Task<CompanyResponseWithDetailsDTO?> GetCompanyByIdAsync(string id);
    }
}
