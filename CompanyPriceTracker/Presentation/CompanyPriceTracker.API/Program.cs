using CompanyPriceTracker.Application.Abstractions.Services;
using CompanyPriceTracker.Application.DTOs.Company;
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
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/// <summary>
/// Yeni Þirket Oluþturma
/// </summary>
/// <param name=""></param>
/// <returns></returns>
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
/// ID ile Þirket Çekme
/// </summary>
/// <param name=""></param>
/// <returns></returns>
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

app.Run();

