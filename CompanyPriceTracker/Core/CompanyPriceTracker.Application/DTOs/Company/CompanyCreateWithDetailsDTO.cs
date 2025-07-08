using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Company {
    public class CompanyCreateWithDetailsDTO {
        public string Name { get; set; } = null!; // eklenecek firmanın adı
        public int Year { get; set; }             // fiyat yılı
        public double Price { get; set; }         // uzman/gün fiyatı
    }
}
