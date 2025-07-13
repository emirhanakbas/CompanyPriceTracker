using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.Authentication {
    public class UserRegisterDTO {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [MinLength(3, ErrorMessage = "Kullanıcı adı en az 3 karakter uzunluğunda olmalıdır.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Şifre gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter uzunluğunda olmalıdır.")]
        public string Password { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string> { "User" };
    }
}
