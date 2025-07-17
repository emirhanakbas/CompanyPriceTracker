using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Authentication {
    public class LoginRequestDTO {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
