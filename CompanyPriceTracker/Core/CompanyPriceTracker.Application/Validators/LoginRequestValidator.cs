using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.DTOs.Authentication;
using FluentValidation;

namespace CompanyPriceTracker.Application.Validators {
    public class LoginRequestValidator : AbstractValidator<LoginRequestDTO> {
        public LoginRequestValidator() {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.");
        }
    }
}
