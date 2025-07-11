using AutoMapper;
using CompanyPriceTracker.Application.DTOs.Company;
using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Domain.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.DTOs.Offer;

namespace CompanyPriceTracker.Application.Profiles {
    public class MappingProfile : Profile { // AutoMapper Profile Class'ı
        public MappingProfile() {
            // Domain Entity -> DTO Mapping'leri
            CreateMap<Company, CompanyResponseWithDetailsDTO>();
            CreateMap<CompanyPrice, CompanyPriceResponseDTO>();

            // DTO -> Domain Entity Mapping'leri
            CreateMap<CompanyCreateWithDetailsDTO, Company>();
            CreateMap<CompanyPriceCreateDTO, CompanyPrice>();

            // Diğer DTO Mapping'leri
            //CreateMap<OfferRequestDTO, ...>();
            //CreateMap<..., OfferResponseDTO >();
        }
    }
}
