using Microsoft.AspNetCore.Mvc;

namespace TeamBuilder.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }

        [HttpPost("test-cors")]
        public IActionResult TestCors([FromBody] object data)
        {
            return Ok(new { 
                message = "CORS test successful",
                receivedData = data,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
