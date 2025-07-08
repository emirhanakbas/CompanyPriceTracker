using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Company {
    public class CompanyResponseWithDetailsDTO {
        public string? Id { get; set; }        // fiyat benzersiz id (mongodb'den gelen)
        public string? Name { get; set; }
        public List<CompanyPriceResponseDTO> Prices { get; set; } = new List<CompanyPriceResponseDTO>(); // tüm fiyat listesi
    }
}
