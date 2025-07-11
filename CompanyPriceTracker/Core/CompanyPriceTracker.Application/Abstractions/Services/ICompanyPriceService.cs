using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Application.DTOs.Offer;
using CompanyPriceTracker.Application.DTOs.ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.Abstractions.Services {
    public interface ICompanyPriceService {
        Task<ServiceResult<CompanyPriceResponseDTO>> AddCompanyPriceAsync(CompanyPriceCreateDTO CompanyPriceDTO);
        Task<ServiceResult<OfferResponseDTO>> CalculateOfferAsync(OfferRequestDTO OfferRequestDTO);
        Task<ServiceResult<IEnumerable<CompanyPriceResponseDTO>>> GetCompanyPricesAsync(string companyId);
    }
}
