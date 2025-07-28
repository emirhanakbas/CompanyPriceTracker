using AutoMapper;
using CompanyPriceTracker.Application.Abstractions.Services; // 'ICompanyPriceService'
using CompanyPriceTracker.Application.DTOs.CompanyPrice;     // CompanyPrice DTOs
using CompanyPriceTracker.Application.DTOs.Offer;            // Offer DTOs
using CompanyPriceTracker.Application.DTOs.ServiceResult;
using CompanyPriceTracker.Domain.Entities;                   // 'CompanyPrice'
using CompanyPriceTracker.Domain.Repositories;               // 'ICompanyPriceRepository'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CompanyPriceTracker.Infrastructure.Services {
    public class CompanyPriceService : ICompanyPriceService {
        private readonly ICompanyPriceRepository _companyPriceRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper; // AutoMapper Injection

        public CompanyPriceService(ICompanyPriceRepository companyPriceRepository, ICompanyRepository companyRepository, IMapper mapper) {
            _companyPriceRepository = companyPriceRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CompanyPriceResponseDTO>> AddCompanyPriceAsync(CompanyPriceCreateDTO companyPriceDto) {
            var existingCompany = await _companyRepository.GetByIdAsync(companyPriceDto.CompanyId!);
            if(existingCompany == null) { // fiyat eklenecek firmanın var olup olmadığını kontrol etme, ileriki geliştirme süreci için
                                          // var olan bir firmaya yeni bir fiyat ataması yapmak için
                return ServiceResult<CompanyPriceResponseDTO>.Failure(error: "Company with ID " + companyPriceDto.CompanyId + " not found.", message: "Fiyat eklemesi için şirket bulunamadı.");
                //throw new ArgumentException($"Company with ID {companyPriceDTO.CompanyId} not found.");
            }
            var existingPrice = await _companyPriceRepository.GetPriceByCompanyAndYearAsync(companyPriceDto.CompanyId!, companyPriceDto.Year);
            if(existingPrice != null) { // eklenecek fiyatın yıl ve firmaya göre kontrolü
                return ServiceResult<CompanyPriceResponseDTO>.Failure(error: "Company price for company ID: '" + companyPriceDto.CompanyId + "' and Year: '" + companyPriceDto.Year + "' already exits.", message: "Belirtilen şirket ve yıla ait fiyat bilgisi bulunmaktadır.");
                //throw new InvalidOperationException($"Company price for company ID {companyPriceDTO.CompanyId} and year {companyPriceDTO.Year} already exits.");
            }
            var companyPrice = _mapper.Map<CompanyPrice>(companyPriceDto); // AutoMapper
            //var companyPrice = new CompanyPrice { // gelen DTO'nun domain entity'sine çevrilmesi // Manuel Mapping
            //    CompanyId = companyPriceDTO.CompanyId!,
            //    Year = companyPriceDTO.Year,
            //    Price = companyPriceDTO.Price
            //};
            await _companyPriceRepository.AddAsync(companyPrice); // repository aracılığıyla veritabanına ekleme
            var responseDTO = _mapper.Map<CompanyPriceResponseDTO>(companyPrice);
            //var responseDTO = new CompanyPriceResponseDTO {
            //    Id = companyPrice.Id,
            //    CompanyId = companyPrice.CompanyId!,
            //    Year = companyPrice.Year,
            //    Price = companyPrice.Price
            //};
            return ServiceResult<CompanyPriceResponseDTO>.Success(responseDTO, "Şirket fiyatı başarıyla eklendi.");
        }

        public async Task<ServiceResult<OfferResponseDTO>> CalculateOfferAsync(OfferRequestDTO offerRequestDTO) {
            if(offerRequestDTO.StartDate > offerRequestDTO.EndDate) {
                return ServiceResult<OfferResponseDTO>.Failure(error: "StartDate cannot be after EndDate.", message: "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }
            int expertDayCount = 0;
            for(DateTime date = offerRequestDTO.StartDate.Date; date <= offerRequestDTO.EndDate.Date; date = date.AddDays(1)) {
                if(date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday ) {
                    expertDayCount++;
                }
            }
            if(expertDayCount == 0) {
                return ServiceResult<OfferResponseDTO>.Failure(error: "No weekdays found in the selected range.", message: "Seçilen tarih aralığında hesaplanacak hafta içi günü bulunmamaktadır. Lütfen hafta içi günleri içeren bir aralık seçin.");
            }
            var company = await _companyRepository.GetByIdAsync(offerRequestDTO.CompanyId);
            if(company == null) {
                return ServiceResult<OfferResponseDTO>.Failure(error: "Company with ID " + offerRequestDTO.CompanyId + " not found.", message: "Teklif hesaplaması için şirket bulunamadı.");
            }
            var companyPrice = await _companyPriceRepository.GetLatestPriceByCompanyIdAsync(offerRequestDTO.CompanyId);
            if (companyPrice == null) {
                return ServiceResult<OfferResponseDTO>.Failure(error: "No price found for company.", message: "Teklif hesaplaması için fiyat bulunamadı.");
            }
            double unitPrice = (double)companyPrice.Price;
            double totalPrice = unitPrice * expertDayCount;
            var responseDto = new OfferResponseDTO {
                CompanyName = company.Name,
                Year = companyPrice.Year,
                ExpertDayCount = expertDayCount,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                Currency = "₺"
            };
            return ServiceResult<OfferResponseDTO>.Success(responseDto, message: "Teklif başarıyla hesaplandı.");
        }

        public async Task<ServiceResult<IEnumerable<CompanyPriceResponseDTO>>> GetCompanyPricesAsync(string companyId) {
            var companyPrices = await _companyPriceRepository.GetPricesByCompanyIdAsync(companyId);
            if(companyPrices == null || !companyPrices.Any()) {
                return ServiceResult<IEnumerable<CompanyPriceResponseDTO>>.Failure(error: "No prices found for company ID " + companyId + ".", message: "Şirket fiyatı bulunmamaktadır.");
            }
            var responseDtos = _mapper.Map<IEnumerable<CompanyPriceResponseDTO>>(companyPrices);
            return ServiceResult<IEnumerable<CompanyPriceResponseDTO>>.Success(responseDtos, "Şirket fiyatları başarıyla alındı.");
        }
    }
}
