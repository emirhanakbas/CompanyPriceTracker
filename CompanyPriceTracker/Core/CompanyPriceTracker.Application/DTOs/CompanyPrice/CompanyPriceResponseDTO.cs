using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.CompanyPrice {
    public class CompanyPriceResponseDTO {
        public string Id { get; set; } = null!;        // fiyat benzersiz id (mongodb'den gelen)
        public string CompanyId { get; set; } = null!; //firma benzersiz id
        public int Year { get; set; }                  // hangi yıla ait fiyat
        public double Price { get; set; }              // uzman/gün fiyatı 
    }
}
