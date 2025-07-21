using AutoMapper;
using CompanyPriceTracker.Application.DTOs.Authentication;
using CompanyPriceTracker.Application.DTOs.ServiceResult;
using CompanyPriceTracker.Domain.Entities;
using CompanyPriceTracker.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyPriceTracker.Application.Abstractions.Services;

namespace CompanyPriceTracker.Infrastructure.Services {
    public class AuthenticationService : IAuthenticationService{
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        
        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper) {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GenerateJwtToken(User user) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Name, user.Username)
            };

            foreach(var role in user.Roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:TokenValidityInHours"] ?? "1")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ServiceResult<LoginResponseDTO>> RegisterAsync(UserRegisterDTO request) {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if(existingUser != null) {
                return ServiceResult<LoginResponseDTO>.Failure(error: "Username already exists.", message: "Bu kullanıcı adı ile kullanıcı bulunmaktadır.");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = _mapper.Map<User>(request);
            user.PasswordHash = passwordHash;
            //var user = new User {
            //    Username = request.Username,
            //    PasswordHash = passwordHash,
            //    Roles = request.Roles ?? new List<string> { "Unauthorized" } // null ise sag taraftaki ifadeyi kullan
            //};
            await _userRepository.AddSync(user);
            var token = GenerateJwtToken(user);
            return ServiceResult<LoginResponseDTO>.Success(new LoginResponseDTO { 
                Token = token, 
                Username = user.Username, 
                Roles = user.Roles 
            }, "User registered successfully.");
        }

        public async Task<ServiceResult<LoginResponseDTO>> LoginAsync(LoginRequestDTO request) {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if(user == null) {
                return ServiceResult<LoginResponseDTO>.Failure(error: "Invalid credentials.", message: "User not found or invalid username/password.");
            }
            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) {
                return ServiceResult<LoginResponseDTO>.Failure(error: "Invalid credentials.", message: "Invalid username/password.");
            }
            var token = GenerateJwtToken(user);
            return ServiceResult<LoginResponseDTO>.Success(new LoginResponseDTO {
                Token = token,
                Username = user.Username,
                Roles = user.Roles
            }, "Login successfull.");
        }
    }
}
