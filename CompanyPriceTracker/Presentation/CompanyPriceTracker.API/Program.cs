using CompanyPriceTracker.Application.Abstractions.Services;
using CompanyPriceTracker.Application.DTOs.Company;
using CompanyPriceTracker.Application.DTOs.CompanyPrice;
using CompanyPriceTracker.Application.DTOs.Offer;
using CompanyPriceTracker.Application.Profiles;
using CompanyPriceTracker.Domain.Repositories;
using CompanyPriceTracker.Infrastructure.Services;
using CompanyPriceTracker.Persistence.Repositories;
using CompanyPriceTracker.Persistence.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<ICompanyRepository, CompanyRepository>();
builder.Services.AddSingleton<ICompanyPriceRepository, CompanyPriceRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyPriceService, CompanyPriceService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/// <summary>
/// Yeni Sirket Oluþturma
/// </summary>
/// <param name="companyDTO">Sirket bilgileri</param>
/// <param name="companyService">Sirket islemleri servisi</param>
/// <returns>
/// Olusturulan sirketin detaylarýný içeren bir HTTP 201 Created yanýtý
/// Basarisiz durumda HTTP 400 Bad Request ve hata mesajlari
/// </returns>
app.MapPost("/api/companies", async (
    [FromBody] CompanyCreateWithDetailsDTO companyDTO, ICompanyService companyService) => {
        var result = await companyService.CreateCompanyAsync(companyDTO);
        if (result.IsSuccess) {
            return Results.CreatedAtRoute("GetCompanyById", new { id = result.Data!.Id }, result.Data);
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
app.MapGet("api/companies/{id}", async (
    [FromRoute] string id, ICompanyService companyService) => {
        var result = await companyService.GetCompanyByIdAsync(id);
        if (result.IsSuccess && result.Data != null) {
            return Results.Ok(result.Data);
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
            return Results.Ok(result.Data);
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
app.MapPost("/api/companyprices", async (
    [FromBody] CompanyPriceCreateDTO companyPriceDTO, ICompanyPriceService companyPriceService) => {
        var result = await companyPriceService.AddCompanyPriceAsync(companyPriceDTO);
        if(result.IsSuccess) {
            return Results.Created("/api/companyprices/" + result.Data!.Id, result.Data);
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
        var result = await companyPriceService.CalculateOfferAsync(requestDTO);

        if (result.IsSuccess) {
            return Results.Ok(result.Data);
        }
        return Results.BadRequest(result.Errors);
    })
    .WithName("CalculateOffer")
    .WithOpenApi(); 

app.Run();

