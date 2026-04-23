using Microsoft.AspNetCore.Mvc;
using NameGen.Core.Models;

namespace NameGen.API.Controllers;

/// <summary>
/// Handles AI enhancement of generated names.
/// </summary>
[ApiController]
[Route("api/v1/names")]
public class EnhanceController : ControllerBase
{
    /// <summary>
    /// Enhances a generated name using AI. Currently stubbed — full OpenAI integration coming in Sprint 3.
    /// </summary>
    /// <param name="request">The name and type to enhance.</param>
    /// <returns>A stub response indicating AI enhancement is coming soon.</returns>
    [HttpGet("enhance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Enhance([FromQuery] EnhanceRequest request)
    {
        return Ok(new EnhanceResponse
        {
            Name        = request.Name,
            Type        = request.Type.ToLower(),
            Enhancement = null,
            Message     = "AI enhancement coming soon."
        });
    }
}