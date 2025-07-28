using CompanyPriceTracker.Application.DTOs.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.Validators {
    public class UserRegisterValidator : AbstractValidator<UserRegisterDTO> {
        public UserRegisterValidator() {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
                .MaximumLength(20).WithMessage("Kullanıcı adı 20 karakteri geçemez.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,100}$").WithMessage("Şifre, 6-100 karakter uzunluğunda olmalı ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.");
            RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("En az bir rol zorunludur.");
        }
    }
}
