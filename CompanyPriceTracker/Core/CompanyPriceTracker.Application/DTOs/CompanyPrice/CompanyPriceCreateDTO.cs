using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.CompanyPrice {
    public class CompanyPriceCreateDTO {
        public string? CompanyId { get; set; } // eklenen firmanın benzersiz ID'si
        public int Year { get; set; }          // eklenen fiyatın hangi yıla ait olduğu
        public double Price { get; set; }      // eklenen firmanın uzman/gün fiyatı
    }
}
