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
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(20).WithMessage("Username must not exceed 20 characters.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,100}$").WithMessage("Password must be 6–100 characters long and include uppercase, lowercase, number, and special character.");
            RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("At least one role is required.");
        }
    }
}
