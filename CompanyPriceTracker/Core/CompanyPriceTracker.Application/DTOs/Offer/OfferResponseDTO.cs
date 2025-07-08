using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Offer {
    public class OfferResponseDTO {
        public string CompanyName { get; set; } = null!; // hesaplanan firmanın adı
        public int Year { get; set; }                    // firmanın hangi yıl fiyatı 
        public int ExpertDayCount { get; set; }          // kaç uzman/gün alınacağı
        public double UnitPrice { get; set; }            // uzman/ gün fiyatı
        public double TotalPrice { get; set; }           // toplam fiyat
        public string Currency { get; set; } = "₺";      // para birimi
    }
}
