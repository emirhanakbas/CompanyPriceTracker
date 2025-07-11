using CompanyPriceTracker.Application.DTOs.Company;
using CompanyPriceTracker.Application.DTOs.ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.Abstractions.Services {
    public interface ICompanyService {
        public Task<ServiceResult<CompanyResponseWithDetailsDTO>> CreateCompanyAsync (CompanyCreateWithDetailsDTO companyDTO);   
        public Task<ServiceResult<IEnumerable<CompanyResponseWithDetailsDTO>>> GetAllCompaniesAsync();
        public Task<ServiceResult<CompanyResponseWithDetailsDTO?>> GetCompanyByIdAsync(string id);
    }
}
