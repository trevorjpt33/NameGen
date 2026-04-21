using Microsoft.AspNetCore.Mvc;

namespace NameGen.API.Controllers;

/// <summary>
/// Handles API health check requests.
/// </summary>
[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the health status of the API.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}