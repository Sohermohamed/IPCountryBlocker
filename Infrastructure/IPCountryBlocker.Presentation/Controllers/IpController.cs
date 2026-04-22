using IPCountryBlocker.Abstraction.IRepositories;
using IPCountryBlocker.Abstraction.IServices;
using IPCountryBlocker.Domain.Entities;
using IPCountryBlocker.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IPCountryBlocker.Presentation.Controllers
{
    [ApiController]
    [Route("api/ip")]
    public class IpController : ControllerBase
    {
        private readonly IIPGeolocationService _ipGeolocationService;
        private readonly CountryService _countryService;
        private readonly ILogRepository _logRepository;

        public IpController(IIPGeolocationService ipGeolocationService, CountryService countryService, ILogRepository logRepository)
        {
            _ipGeolocationService = ipGeolocationService;
            _countryService = countryService;
            _logRepository = logRepository;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
        {
            var targetIp = string.IsNullOrWhiteSpace(ipAddress) 
                ? HttpContext.Connection.RemoteIpAddress?.ToString() 
                : ipAddress;

            if (string.IsNullOrEmpty(targetIp))
                return BadRequest(new { Message = "Could not determine IP Address." });

            if (!IPAddress.TryParse(targetIp, out _))
                return BadRequest(new { Message = "Invalid IP Address format." });

            var ipInfo = await _ipGeolocationService.GetIPInfoAsync(targetIp);
            
            if (ipInfo == null)
                return StatusCode(502, new { Message = "Failed to retrieve IP geolocation data from external provider." });

            return Ok(ipInfo);
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var callerIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            
            var ipInfo = await _ipGeolocationService.GetIPInfoAsync(callerIp);
            if (ipInfo == null)
                return StatusCode(502, new { Message = "Could not verify geolocation. External provider failed." });

            var isBlocked = _countryService.IsCountryBlocked(ipInfo.CountryCode);

            var log = new BlockedAttemptLog
            {
                IPAddress = callerIp,
                Timestamp = DateTime.UtcNow,
                CountryCode = ipInfo.CountryCode ?? "N/A",
                IsBlocked = isBlocked,
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            _logRepository.Add(log);

            return Ok(new 
            { 
                IPAddress = callerIp, 
                CountryResult = ipInfo, 
                IsBlocked = isBlocked 
            });
        }
    }
}
