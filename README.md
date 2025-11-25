# Bank Web Admin API

ASP.NET Core Web API for bank administration with currency exchange rates.

## Features

- Currency exchange rates API
- Azure AD authentication support
- Swagger/OpenAPI documentation

## Prerequisites

- .NET 6.0 SDK
- Azure AD tenant (optional, for authentication)

## Setup

```bash
cd WebAdminAPI
dotnet restore
dotnet run
```

Access Swagger UI at: `https://localhost:5001/swagger`

## Docker

```bash
docker build -t bank-admin-api .
docker run -p 5000:80 bank-admin-api
```

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/currency` | GET | Get all currency rates |
| `/api/currency/{name}` | GET | Get rate for specific currency |

## Supported Currencies

USD, EUR, GBP, JPY, CHF, CAD, AUD, CNY, INR, BRL

## Configuration

For Azure AD authentication, update `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-domain.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id"
  }
}
```
