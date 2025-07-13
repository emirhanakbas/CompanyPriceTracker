using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Authentication {
    public class LoginResponseDTO {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
