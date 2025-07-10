using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.Abstractions.Services; // 'ICompanyPriceService'
using CompanyPriceTracker.Application.DTOs.CompanyPrice;     // CompanyPrice DTOs
using CompanyPriceTracker.Application.DTOs.Offer;            // Offer DTOs
using CompanyPriceTracker.Application.DTOs.ServiceResult;
using CompanyPriceTracker.Domain.Entities;                   // 'CompanyPrice'
using CompanyPriceTracker.Domain.Repositories;               // 'ICompanyPriceRepository'

namespace CompanyPriceTracker.Infrastructure.Services {
    public class CompanyPriceService : ICompanyPriceService {
        private readonly ICompanyPriceRepository _companyPriceRepository;
        private readonly ICompanyRepository _companyRepository;

        public CompanyPriceService(ICompanyPriceRepository companyPriceRepository, ICompanyRepository companyRepository) {
            _companyPriceRepository = companyPriceRepository;
            _companyRepository = companyRepository;
        }

        public async Task<ServiceResult<CompanyPriceResponseDTO>> AddCompanyPriceAsync(CompanyPriceCreateDTO companyPriceDTO) {
            var existingCompany = await _companyRepository.GetByIdAsync(companyPriceDTO.CompanyId);
            
            if(existingCompany == null) { // fiyat eklenecek firmanın var olup olmadığını kontrol etme
                return ServiceResult<CompanyPriceResponseDTO>.Failure(error: "Company with ID " + companyPriceDTO.CompanyId + " not found.", message: "Company not found for price addition.");
                //throw new ArgumentException($"Company with ID {companyPriceDTO.CompanyId} not found.");
            }
            
            var existingPrice = await _companyPriceRepository.GetPriceByCompanyAndYearAsync(companyPriceDTO.CompanyId, companyPriceDTO.Year);
            
            if(existingPrice != null) { // eklenecek fiyatın yıl ve firmaya göre kontrolü
                return ServiceResult<CompanyPriceResponseDTO>.Failure(error: "Company price for company ID " + companyPriceDTO.CompanyId + " and year" + companyPriceDTO.Year + " already exits.");
                //throw new InvalidOperationException($"Company price for company ID {companyPriceDTO.CompanyId} and year {companyPriceDTO.Year} already exits.");
            }
            
            var companyPrice = new CompanyPrice { // gelen DTO'nun domain entity'sine çevrilmesi
                CompanyId = companyPriceDTO.CompanyId,
                Year = companyPriceDTO.Year,
                Price = companyPriceDTO.Price
            };

            await _companyPriceRepository.AddAsync(companyPrice); // repository aracılığıyla veritabanına ekleme

            var responseDTO = new CompanyPriceResponseDTO {
                Id = companyPrice.Id,
                CompanyId = companyPrice.CompanyId,
                Year = companyPrice.Year,
                Price = companyPrice.Price
            };

            return ServiceResult<CompanyPriceResponseDTO>.Success(responseDTO, "Company price added successfully.");
        }

        public async Task<ServiceResult<OfferResponseDTO>> CalculateOfferAsync(OfferRequestDTO offerRequestDTO) {
            //    var company = await _companyRepository.GetByIdAsync(offerRequestDTO.CompanyId);
            //    if (company == null) {
            //        throw new ArgumentException($"Company with ID {offerRequestDTO.CompanyId} not found.");
            //    }
            //    var companyPrice = await _companyPriceRepository.GetLatestPriceByCompanyIdAsync(offerRequestDTO.CompanyId);
            //    if (companyPrice == null) {
            //        throw new InvalidOperationException($"No price found for company '{company.Name}' (ID: {offerRequestDTO.CompanyId}. Cannot calculate offer.");
            //    }
            //    double unitPrice = (double)companyPrice.Price;
            //    double totalPrice = unitPrice * offerRequestDTO.ExpertDayCount;

            //}
        }

        public async Task<ServiceResult<IEnumerable<CompanyPriceResponseDTO>>> GetCompanyPricesAsync(string companyId) {
            
        }
    }
}
