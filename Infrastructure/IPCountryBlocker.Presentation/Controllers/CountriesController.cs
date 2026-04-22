using IPCountryBlocker.Service.Services;
using IPCountryBlocker.Shared.DTO_s;
using Microsoft.AspNetCore.Mvc;

namespace IPCountryBlocker.Presentation.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly CountryService _countryService;

        public CountriesController(CountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _countryService.BlockCountry(request.CountryCode, request.CountryName);

            if (!result)
                return Conflict(new { Message = "Country already blocked or invalid code." });

            return Ok(new { Message = "Country blocked successfully." });
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            var result = _countryService.UnblockCountry(countryCode);

            if (!result)
                return NotFound(new { Message = "Country not found in blocked list." });

            return Ok(new { Message = "Country unblocked successfully." });
        }

        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 || pageSize > 100 ? 10 : pageSize;

            var result = _countryService.GetBlockedCountries(page, pageSize, search);
            return Ok(new { Page = page, PageSize = pageSize, Data = result });
        }

        [HttpPost("temporal-block")]
        public IActionResult TemporalBlock([FromBody] TemporalBlockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _countryService.BlockCountryTemporarily(request.CountryCode, request.DurationMinutes);
            
            if (!result) 
                return Conflict(new { Message = "Country is already temporarily blocked or invalid." });
                
            return Ok(new { Message = $"Country temporarily blocked for {request.DurationMinutes} minutes." });
        }
    }
}
