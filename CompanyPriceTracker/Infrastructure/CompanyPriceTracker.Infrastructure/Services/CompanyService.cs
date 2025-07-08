using CompanyPriceTracker.Application.Abstractions.Services; // 'ICompanyService'
using CompanyPriceTracker.Application.DTOs.Company;          // Company DTOs
using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Domain.Entities;                   // 'Company'
using CompanyPriceTracker.Domain.Repositories;               // 'ICompanyRepository'
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Infrastructure.Services {
    public class CompanyService : ICompanyService{
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyPriceService _companyPriceService;

        public CompanyService(ICompanyRepository companyRepository, ICompanyPriceService companyPriceService) { // DEPENDENCY INVERSION
            _companyRepository = companyRepository;
            _companyPriceService = companyPriceService;
        }

        public async Task<CompanyResponseWithDetailsDTO> CreateCompanyAsync(CompanyCreateWithDetailsDTO companyDTO) { // AutoMapper kütüphanesi ile otomatik yapılabilir
            var company = new Company { // AutoMapper kesin yap
                Name = companyDTO.Name // DTO'dan gelen ad entity'e atanıyor; Id mongodb tarafından otomatik olarak oluşturulacak
            };
            await _companyRepository.AddAsync(company); // domain entity'si repository aracılığı ile veritabanına eklenir
            
            var companyPriceWithYear = new CompanyPriceCreateDTO {
                CompanyId = company.Id!,
                Year = companyDTO.Year,
                Price = companyDTO.Price
            };
            // serviceresult generic class tüm dönüşler böyle olacak 
            var createdPriceResponse = await _companyPriceService.AddCompanyPriceAsync(companyPriceWithYear);
            
            return new CompanyResponseWithDetailsDTO {  // dış dünyaya dönecek CompanyResponseDTO oluyor
                Id = company.Id,
                Name = company.Name,
                Prices = new List<CompanyPriceResponseDTO> { createdPriceResponse }
            };
        }
        //ilist ienum farkları
        public async Task<IEnumerable<CompanyResponseWithDetailsDTO>> GetAllCompaniesAsync() {
            var companies = await _companyRepository.GetAllAsync(); // repository aracılığı ile tüm company entity'leri veritabanından çekilir
            return companies.Select(company => new CompanyResponseWithDetailsDTO { // Select LINQ
                Id = company.Id,
                Name = company.Name
            });
        }

        public async Task<CompanyResponseWithDetailsDTO?> GetCompanyByIdAsync(string id) {
            var company = await _companyRepository.GetByIdAsync(id); // repository'den id ile çekiliyor
            
            if(company == null) {
                return null;
            }

            var prices = await _companyPriceService.GetCompanyPricesAsync(company.Id!);

            return new CompanyResponseWithDetailsDTO { // bulunan domain entity CompanyResponseDTO'ya çevrilip döndürülüyor
                Id = company.Id,
                Name = company.Name,
                Prices = prices.ToList()
            };
        }
    }
}
