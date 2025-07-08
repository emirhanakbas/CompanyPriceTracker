using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.Abstractions.Services; // 'ICompanyPriceService'
using CompanyPriceTracker.Application.DTOs.CompanyPrice;     // CompanyPrice DTOs
using CompanyPriceTracker.Application.DTOs.Offer;            // Offer DTOs
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

        public async Task<CompanyPriceResponseDTO> AddCompanyPriceAsync(CompanyPriceCreateDTO companyPriceDTO) {
            var existingCompany = await _companyRepository.GetByIdAsync(companyPriceDTO.CompanyId);
            if(existingCompany == null) { // fiyat eklenecek firmanın var olup olmadığını kontrol etme
                throw new ArgumentException($"Company with ID {companyPriceDTO.CompanyId} not found.");
            }

            var existingPrice = await _companyPriceRepository.GetPriceByCompanyAndYearAsync(companyPriceDTO.CompanyId, companyPriceDTO.Year);
            if(existingPrice != null) { // eklenecek fiyatın yıl ve firmaya göre kontrolü
                throw new InvalidOperationException($"Company price for company ID {companyPriceDTO.CompanyId} and year {companyPriceDTO.Year} already exits.");
            }

            var companyPrice = new CompanyPrice { // gelen DTO'nun domain entity'sine çevrilmesi
                CompanyId = companyPriceDTO.CompanyId,
                Year = companyPriceDTO.Year,
                Price = companyPriceDTO.Price
            };

            await _companyPriceRepository.AddAsync(companyPrice); // repository aracılığıyla veritabanına ekleme
            return new CompanyPriceResponseDTO { //veritabanına eklenen entity'i CompanyPriceResponseDTO'su olarak döndürme
                Id = companyPrice.Id,
                CompanyId = companyPrice.CompanyId,
                Year = companyPrice.Year,
                Price = companyPrice.Price
            };
        }
    
        public async Task<OfferResponseDTO> CalculateOfferAsync(OfferRequestDTO offerRequestDTO) {
            var company = await _companyRepository.GetByIdAsync(offerRequestDTO.CompanyId);
            if (company == null) {
                throw new ArgumentException($"Company with ID {offerRequestDTO.CompanyId} not found.");
            }
            var companyPrice = await _companyPriceRepository.GetLatestPriceByCompanyIdAsync(offerRequestDTO.CompanyId);
            if (companyPrice == null) {
                throw new InvalidOperationException($"No price found for company '{company.Name}' (ID: {offerRequestDTO.CompanyId}. Cannot calculate offer.");
            }
            double unitPrice = (double)companyPrice.Price;
            double totalPrice = unitPrice * offerRequestDTO.ExpertDayCount;
            return new OfferResponseDTO {
                ExpertDayCount = offerRequestDTO.ExpertDayCount,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                Currency = "₺"
            };
        }

        public async Task<IEnumerable<CompanyPriceResponseDTO>> GetCompanyPricesAsync(string companyId) {
            var companyPrices = await _companyPriceRepository.GetPricesByCompanyIdAsync(companyId);
            return companyPrices.Select(companyPrices => new CompanyPriceResponseDTO {
                Id = companyPrices.Id,
                CompanyId = companyPrices.CompanyId,
                Year = companyPrices.Year,
                Price = companyPrices.Price
            }).OrderByDescending(companyPrices => companyPrices.Year);
        }
    }
}
