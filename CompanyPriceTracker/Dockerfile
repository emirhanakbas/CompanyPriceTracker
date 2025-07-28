FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Presentation/CompanyPriceTracker.API/CompanyPriceTracker.API.csproj", "Presentation/CompanyPriceTracker.API/"]
COPY ["Core/CompanyPriceTracker.Application/CompanyPriceTracker.Application.csproj", "Core/CompanyPriceTracker.Application/"]
COPY ["Infrastructure/CompanyPriceTracker.Infrastructure/CompanyPriceTracker.Infrastructure.csproj", "Infrastructure/CompanyPriceTracker.Infrastructure/"]
COPY ["Infrastructure/CompanyPriceTracker.Persistence/CompanyPriceTracker.Persistence.csproj", "Infrastructure/CompanyPriceTracker.Persistence/"]


RUN dotnet restore "Presentation/CompanyPriceTracker.API/CompanyPriceTracker.API.csproj"
COPY . . 
WORKDIR "/src/Presentation/CompanyPriceTracker.API"
RUN dotnet build "CompanyPriceTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Presentation/CompanyPriceTracker.API
RUN dotnet publish "CompanyPriceTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "CompanyPriceTracker.API.dll"]