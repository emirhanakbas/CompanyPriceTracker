using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Authentication {
    public class LoginRequestDTO {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Şifre gereklidir.")]
        public string Password { get; set; } = null!;
    }
}
