using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Application.DTOs.Offer;

namespace CompanyPriceTracker.Application.Abstractions.Services {
    public interface ICompanyPriceService {
        Task<CompanyPriceResponseDTO> AddCompanyPriceAsync(CompanyPriceCreateDTO CompanyPriceDTO);
        Task<IEnumerable<CompanyPriceResponseDTO>> GetCompanyPricesAsync(string companyId);
        Task<OfferResponseDTO> CalculateOfferAsync(OfferRequestDTO OfferRequestDTO);
    }
}
