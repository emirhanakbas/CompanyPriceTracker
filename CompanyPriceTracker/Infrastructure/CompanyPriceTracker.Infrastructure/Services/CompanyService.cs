using CompanyPriceTracker.Application.Abstractions.Services; // 'ICompanyService'
using CompanyPriceTracker.Application.DTOs.Company;          // Company DTOs
using CompanyPriceTracker.Application.DTOs.CompanyPrice;     // CompanyPriceDTOs
using CompanyPriceTracker.Application.DTOs.ServiceResult;    // 'ServiceResult'
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

        public async Task<ServiceResult<CompanyResponseWithDetailsDTO>> CreateCompanyAsync(CompanyCreateWithDetailsDTO companyDTO) { // AutoMapper kütüphanesi ile otomatik yapılabilir
            var company = new Company { // AutoMapper kesin yap
                Name = companyDTO.Name // DTO'dan gelen ad entity'e atanıyor; Id MongoDB tarafından otomatik olarak oluşturulacak
            };
            await _companyRepository.AddAsync(company); // domain entity'si repository aracılığı ile veritabanına eklenir
            
            var companyPriceWithYear = new CompanyPriceCreateDTO {
                CompanyId = company.Id!,
                Year = companyDTO.Year,
                Price = companyDTO.Price
            };
           
            var createdPriceResponse = await _companyPriceService.AddCompanyPriceAsync(companyPriceWithYear);
            if(!createdPriceResponse.IsSuccess) {
                return ServiceResult<CompanyResponseWithDetailsDTO>.Failure(message: "Company created but price addition failed.", errors: createdPriceResponse.Errors);
            }

            var responseDto = new CompanyResponseWithDetailsDTO {
                Id = company.Id,
                Name = company.Name,
                Prices = new List<CompanyPriceResponseDTO> { createdPriceResponse.Data! }
            };
            return ServiceResult<CompanyResponseWithDetailsDTO>.Success(responseDto, "Company and initial price created successfully.");
        }

        public async Task<IEnumerable<CompanyResponseWithDetailsDTO>> GetAllCompaniesAsync() {
            var companies = await _companyRepository.GetAllAsync(); // repository aracılığı ile tüm company entity'leri veritabanından çekilir
            return companies.Select(company => new CompanyResponseWithDetailsDTO { // Select LINQ
                Id = company.Id,
                Name = company.Name
            });
        }

        public async Task<ServiceResult<CompanyResponseWithDetailsDTO?>> GetCompanyByIdAsync(string id) {
            var company = await _companyRepository.GetByIdAsync(id); // repository'den id ile çekiliyor
            if(company == null) {
                return ServiceResult<CompanyResponseWithDetailsDTO?>.Failure($"Company with ID {id} not found.", "Company not found.");
            }

            var prices = await _companyPriceService.GetCompanyPricesAsync(company.Id!);
            if(!prices.IsSuccess) {
                return ServiceResult<CompanyResponseWithDetailsDTO?>.Failure(message: "Company found but failed to retrieve prices.", errors: prices.Errors);
            }

            var responseDTO = new CompanyResponseWithDetailsDTO {
                Id = company.Id,
                Name = company.Name,
                Prices = prices.Data!.ToList()
            };
            return ServiceResult<CompanyResponseWithDetailsDTO?>.Success(responseDTO, "Company details retrieved successfully.");
        }
    }
}
