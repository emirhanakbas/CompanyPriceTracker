using CompanyPriceTracker.API.Filters;
using CompanyPriceTracker.Application.Abstractions.Services;
using CompanyPriceTracker.Application.DTOs.Authentication;
using CompanyPriceTracker.Application.DTOs.Company;
using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Application.DTOs.Offer;
using CompanyPriceTracker.Application.Profiles;
using CompanyPriceTracker.Application.Validators;
using CompanyPriceTracker.Domain.Repositories;
using CompanyPriceTracker.Infrastructure.Services;
using CompanyPriceTracker.Persistence.Repositories;
using CompanyPriceTracker.Persistence.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>(); // Ýçinde bulunduðu assembly'deki tüm validator'larý çeker
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<ICompanyRepository, CompanyRepository>();
builder.Services.AddSingleton<ICompanyPriceRepository, CompanyPriceRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyPriceService, CompanyPriceService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "CompanyPriceTrackerAPI", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy => {
            policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Yeni Sirket Oluþturma
/// </summary>
/// <param name="companyDTO">Sirket bilgileri</param>
/// <param name="companyService">Sirket islemleri servisi</param>
/// <returns>
/// Olusturulan sirketin detaylarýný içeren bir HTTP 201 Created yanýtý
/// Basarisiz durumda HTTP 400 Bad Request ve hata mesajlari
/// </returns>
app.MapPost("/api/companies", [Authorize(Roles = "User, Admin")] async (
    [FromBody] CompanyCreateWithDetailsDTO companyDTO, ICompanyService companyService) => {
        var result = await companyService.CreateCompanyAsync(companyDTO);
        if (result.IsSuccess) {
            return Results.Created("/api/companies/" + result.Data!.Id, result);
        } else {
            return Results.BadRequest(result.Errors);
        }
    })
    .WithName("CreateCompany")
    .WithOpenApi();

/// <summary>
/// ID ile Sirket Getirme
/// </summary>
/// <param name="id">Sirket ID'si</param>
/// <param name="companyService">Sirket islemleri servisi</param>
/// <returns>
/// Belirtilen ID'ye sahip sirketin detaylarini iceren bir HTTP 200 OK yaniti
/// Gecersiz istek durumunda HTTP 400 Bad Request ve hata mesajlari 
/// </returns>
app.MapGet("api/companies/{id}", [Authorize(Roles = "User, Admin")] async (
    [FromRoute] string id, ICompanyService companyService) => {
        var result = await companyService.GetCompanyByIdAsync(id);
        if (result.IsSuccess && result.Data != null) {
            return Results.Ok(result);
        } else if (result.IsSuccess && result.Data == null) {
            return Results.NotFound(result.Errors);
        } else {
            return Results.BadRequest(result.Errors);
        }
    })
    .WithName("GetCompanyById")
    .WithOpenApi();

/// <summary>
/// Tum Sirketleri Getirme
/// </summary>
/// <param name="companyService">Sirket islemleri servisi</param>
/// <returns>
/// Tum sirketlerin listesini iceren bir HTTP 200 OK yaniti
/// Sirket bulunamazsa (liste bossa) HTTP 404 Not Found ve hata mesajlari
/// </returns>
app.MapGet("api/companies", async (
    ICompanyService companyService) => {
        var result = await companyService.GetAllCompaniesAsync();
        if(result.IsSuccess) {
            return Results.Ok(result);
        }
        return Results.NotFound(result.Errors);
    })
    .WithName("GetAllCompanies")
    .WithOpenApi();

/// <summary>
/// Sirket Fiyatý Ekleme
/// </summary>
/// <param name="companyPriceDTO">Sirket bilgileri</param>
/// <param name="companyPriceService">Sirket fiyat islemleri servisi</param>
/// <returns>
/// Olusturulan sirket fiyatinin detaylarini icrene bir HTTP 201 Created yaniti
/// Basarisiz durumda HTTP 400 Bad Request ve hata mesajlari
/// </returns>
app.MapPost("/api/companyprices", [Authorize(Roles = "User, Admin")] async (
    [FromBody] CompanyPriceCreateDTO companyPriceDTO, ICompanyPriceService companyPriceService) => {
        var result = await companyPriceService.AddCompanyPriceAsync(companyPriceDTO);
        if(result.IsSuccess) {
            return Results.Created("/api/companyprices/" + result.Data!.Id, result);
        }
        return Results.BadRequest(result.Errors);
    })
    .WithName("AddCompanyPrice")
    .WithOpenApi();

/// <summary>
/// Teklif Hesaplama
/// </summary>
/// <param name="requestDTO">Teklif hesaplama için sirket ID ve ay süresi bilgileri</param>
/// <param name="companyPriceService">Sirket fiyat islemleri servisi</param>
/// <returns>
/// Hesaplanan teklif miktarini iceren bir HTTP 200 OK yaniti
/// Basarisiz durumda HTTP 400 Bad Request ve hata mesajlari
/// </returns>
app.MapPost("/api/offers/calculate", async (
    [FromBody] OfferRequestDTO requestDTO, ICompanyPriceService companyPriceService) => {
        Console.WriteLine($"DTO: companyId={requestDTO.CompanyId}, startDate={requestDTO.StartDate.ToShortDateString()}, endDate={requestDTO.EndDate.ToShortDateString()}");
        var result = await companyPriceService.CalculateOfferAsync(requestDTO);

        if (result.IsSuccess) {
            return Results.Ok(result);
        }
        return Results.BadRequest(result.Errors);
    })
    .WithName("CalculateOffer")
    .WithOpenApi();

/// <summary>
/// Yeni kullanýcý kaydý yapar (Sadece Admin)
/// </summary>
/// <param name="request">Kullanýcý kayýt bilgileri</param>
/// <param name="authService">Auth servisi</param>
/// <returns>Kayýt baþarýlý olursa token ve kullanýcý bilgileri</returns>
app.MapPost("/api/auth/register", [Authorize(Roles = "Admin")] async ( // Sadece Adminler yeni kullanýcý kaydedebilir
    [FromBody] UserRegisterDTO request, IAuthenticationService authService) => {
        var result = await authService.RegisterAsync(request);
        if(result.IsSuccess) {
            return Results.Ok(result);
        }
        return Results.BadRequest(result.Errors);
    })
    .AddEndpointFilter<ValidationFilter<UserRegisterDTO>>()
    .WithName("RegisterUser")
    .WithOpenApi();

/// <summary>
/// Kullanici girisi yapar ve JWT token döndürür
/// </summary>
/// <param name="request">Kullanici giris bilgileri</param>
/// <param name="authService">Auth servisi</param>
/// <returns>Basarili giris olursa token ve kullanici bilgileri</returns>
app.MapPost("/api/auth/login", async ( // Herkes giris yapabilir (Authorize deðil)
    [FromBody] LoginRequestDTO request, IAuthenticationService authService) => { // IAuthenticationService
        var result = await authService.LoginAsync(request);
        if (result.IsSuccess) {
            return Results.Ok(result);
        }
        return Results.Unauthorized(); // Basarisiz olursa Unauthorized döner
    })
    .AddEndpointFilter<ValidationFilter<LoginRequestDTO>>()
    .WithName("LoginUser")
    .WithOpenApi();

app.Run();

