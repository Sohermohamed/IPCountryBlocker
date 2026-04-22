using IPCountryBlocker.Abstraction.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IPCountryBlocker.Presentation.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogRepository _logRepository;

        public LogsController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var logs = _logRepository.GetAll()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(logs);
        }
    }
}
