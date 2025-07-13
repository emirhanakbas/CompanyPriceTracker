using CompanyPriceTracker.Application.DTOs.Authentication;
using CompanyPriceTracker.Application.DTOs.ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.Abstractions.Services {
    public interface IAuthenticationService {
        Task<ServiceResult<LoginResponseDTO>> RegisterAsync(UserRegisterDTO request);
        Task<ServiceResult<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
    }
}
