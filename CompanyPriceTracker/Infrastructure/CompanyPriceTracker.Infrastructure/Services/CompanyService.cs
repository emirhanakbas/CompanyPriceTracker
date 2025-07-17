using CompanyPriceTracker.Application.Abstractions.Services; // 'ICompanyService'
using CompanyPriceTracker.Application.DTOs.Company;          // Company DTOs
using CompanyPriceTracker.Application.DTOs.CompanyPrice;     // CompanyPriceDTOs
using CompanyPriceTracker.Application.DTOs.ServiceResult;    // 'ServiceResult'
using CompanyPriceTracker.Domain.Entities;                   // 'Company'
using CompanyPriceTracker.Domain.Repositories;               // 'ICompanyRepository'
using AutoMapper;
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
        private readonly IMapper _mapper; // IMapper Injection

        public CompanyService(ICompanyRepository companyRepository, ICompanyPriceService companyPriceService, IMapper mapper) { // DEPENDENCY INVERSION
            _companyRepository = companyRepository;
            _companyPriceService = companyPriceService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CompanyResponseWithDetailsDTO>> CreateCompanyAsync(CompanyCreateWithDetailsDTO companyDTO) {
            Company? company = await _companyRepository.GetByNameAsync(companyDTO.Name);
            bool isNewCompany = false;
            if (company == null) {
                company = _mapper.Map<Company>(companyDTO); // companyDTO -> CompanyEntity Mapping
                await _companyRepository.AddAsync(company); // domain entity'si repository aracılığı ile veritabanına eklenir
                isNewCompany = true;
            } else {
                Console.WriteLine("Company " + company.Name + " already exists with ID: " + company.Id + ".");
            }
            var companyPriceWithYear = new CompanyPriceCreateDTO {
                CompanyId = company.Id!,
                Year = companyDTO.Year,
                Price = companyDTO.Price
            }; // AutoMapper ile nasıl yapılır?
            //var companyPriceWithYear = _mapper.Map<CompanyPriceCreateDTO>(companyDTO);
            //companyPriceWithYear.CompanyId = company.Id!;
            var createdPriceResponse = await _companyPriceService.AddCompanyPriceAsync(companyPriceWithYear);
            if(!createdPriceResponse.IsSuccess) {
                Console.WriteLine("Company created but price addition failed.");
                return ServiceResult<CompanyResponseWithDetailsDTO>.Failure(message: "Company created but price addition failed.", errors: createdPriceResponse.Errors);
            }
            var responseDto = _mapper.Map<CompanyResponseWithDetailsDTO>(company); // Company Entity -> CompanyResponseWithDetailsDTO Mapping
            /*var responseDto = new CompanyResponseWithDetailsDTO { // Manuel Map
                Id = company.Id,
                Name = company.Name,
                Prices = new List<CompanyPriceResponseDTO> { createdPriceResponse.Data! }
            };*/
            if(createdPriceResponse.Data != null) {
                responseDto.Prices.Add(createdPriceResponse.Data);
            }
            string successMessage = isNewCompany ? "New company and initial price created successfully." : "Price added to existing company successfully.";
            return ServiceResult<CompanyResponseWithDetailsDTO>.Success(responseDto, successMessage);
        }

        public async Task<ServiceResult<IEnumerable<CompanyResponseWithDetailsDTO>>> GetAllCompaniesAsync() {
            var companies = await _companyRepository.GetAllAsync(); // repository aracılığı ile tüm company entity'leri veritabanından çekilir
            var responseDTOs = new List<CompanyResponseWithDetailsDTO>();
            foreach(var company in companies) {
                var companyDTO = _mapper.Map<CompanyResponseWithDetailsDTO>(company);
                var prices = await _companyPriceService.GetCompanyPricesAsync(company.Id!);
                if(prices.IsSuccess && prices.Data! != null) {
                    companyDTO.Prices = prices.Data.ToList();
                } else {
                    companyDTO.Prices = new List<CompanyPriceResponseDTO>();
                }
                responseDTOs.Add(companyDTO);
            }
            //var responseDtos = _mapper.Map<IEnumerable<CompanyResponseWithDetailsDTO>>(companies);
            /*return companies.Select(company => new CompanyResponseWithDetailsDTO { // Select LINQ
                Id = company.Id,
                Name = company.Name
            });*/
            return ServiceResult<IEnumerable<CompanyResponseWithDetailsDTO>>.Success(responseDTOs, "All companies retrieved successfully.");
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

            var responseDto = _mapper.Map<CompanyResponseWithDetailsDTO>(company); // Company Entity -> CompanyResponseWithDetailsDTO Mapping
            /*var responseDTO = new CompanyResponseWithDetailsDTO {
                Id = company.Id,
                Name = company.Name,
                Prices = prices.Data!.ToList()
            };*/
            if (prices.Data != null) {
                responseDto.Prices = prices.Data.ToList(); 
            } else {
                responseDto.Prices = new List<CompanyPriceResponseDTO>(); 
            }
            return ServiceResult<CompanyResponseWithDetailsDTO?>.Success(responseDto, message: "Company details retrieved successfully.");
        }
    }
}
