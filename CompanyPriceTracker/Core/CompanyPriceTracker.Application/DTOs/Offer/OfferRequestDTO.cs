using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Offer {
    public class OfferRequestDTO {
        public string CompanyId { get; set; } = null!; // teklif alınacak firmanın benzersiz ID'si
        public int ExpertDayCount { get; set; }        // ihtiyaç duyulan uzman/gün sayısı 
    }
}
