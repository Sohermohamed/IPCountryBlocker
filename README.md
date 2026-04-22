# IPCountryBlocker API

A .NET 9 Web API for managing blocked countries and validating IP addresses using third-party geolocation services.

## Architecture & Choices
- **Clean Architecture Principles:** Separation of concerns using Services, Interfaces, and Repositories.
- **In-Memory Storage:** `ConcurrentDictionary` and `ConcurrentQueue` are utilized inside singleton repository classes to ensure thread safety without relying on a database.
- **HttpClientFactory:** Configured to handle API calls to `ipapi.co` efficiently, preventing socket exhaustion.
- **Hosted Services:** `BackgroundService` runs every 5 minutes to asynchronously clear expired temporal country blocks without freezing the main application threads.

## How to Run
1. Clone the repository and navigate to the root directory.
2. Ensure you have the .NET 9 SDK installed.
3. Open `appsettings.json` and insert your API key for `ipapi.co` in `GeoLocationApi:ApiKey`.
4. Run the project using Visual Studio (hit F5) or CLI:
   ```bash
   dotnet run
5. Navigate to `https://localhost:<port>/swagger` to view and interact with the endpoints.

## Endpoints Summary

* `POST /api/countries/block` - Permanently block a country (e.g., `{"countryCode": "US"}`).

* `DELETE /api/countries/block/{countryCode}` - Remove a country from the block list.

* `GET /api/countries/blocked` - Retrieve paginated, searchable blocked countries.

* `POST /api/countries/temporal-block` - Block a country for a specific duration in minutes (1-1440).

* `GET /api/ip/lookup?ipAddress={ip}` - Look up an IP using the third-party provider (uses caller IP if omitted).

* `GET /api/ip/check-block` - Checks if the caller IP is blocked based on its country, and logs the attempt.

* `GET /api/logs/blocked-attempts` - Retrieve a paginated list of all block-check attempts.

---
