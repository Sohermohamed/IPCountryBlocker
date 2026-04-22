using IPCountryBlocker.Abstraction.IServices;
using IPCountryBlocker.Shared.DTO_s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace IPCountryBlocker.Service.Services
{
    public class IPGeolocationService : IIPGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly ILogger<IPGeolocationService> _logger;

        public IPGeolocationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<IPGeolocationService> logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["IpApi:BaseUrl"] ?? "https://ipapi.co/";
            _apiKey = configuration["IpApi:ApiKey"] ?? string.Empty;
            _logger = logger;

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "IPCountryBlocker/1.0");
            }
        }

        public async Task<IPInfoDto?> GetIPInfoAsync(string ipAddress)
        {
            try
            {
                // Handle Localhost for testing
                if (ipAddress == "127.0.0.1" || ipAddress == "::1")
                {
                    return new IPInfoDto 
                    { 
                        CountryCode = "EG", // Default to Egypt for local testing
                        CountryName = "Egypt", 
                        ISP = "Localhost" 
                    };
                }

                if (!IPAddress.TryParse(ipAddress, out _))
                {
                    _logger.LogWarning("Invalid IP format: {Ip}", ipAddress);
                    return null;
                }

                // Build URL properly for ipapi.co
                var url = $"{_baseUrl}{ipAddress}/json/";

                if (!string.IsNullOrWhiteSpace(_apiKey) && _apiKey != "YOUR_API_KEY")
                {
                    url += $"?key={_apiKey}";
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        _logger.LogError("IP API rate limit exceeded for IP {Ip}", ipAddress);
                    }
                    else
                    {
                        _logger.LogError("IP API failed with status {StatusCode} for IP {Ip}",
                            response.StatusCode, ipAddress);
                    }

                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<IPInfoDto>(content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result == null || string.IsNullOrWhiteSpace(result.CountryCode))
                {
                    _logger.LogWarning("Invalid response from IP API for IP {Ip}. Response: {Content}", ipAddress, content);
                    return null;
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during IP lookup for {Ip}", ipAddress);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during IP lookup for {Ip}", ipAddress);
                return null;
            }
        }
    }
}