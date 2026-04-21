using Microsoft.AspNetCore.Mvc;
using NameGen.Core.Interfaces;
using NameGen.Core.Models;

namespace NameGen.API.Controllers;

/// <summary>
/// Handles name generation requests.
/// </summary>
[ApiController]
[Route("api/v1/names")]
public class NamesController : ControllerBase
{
    private readonly IHumanNameService _humanNameService;

    /// <summary>
    /// Initializes a new instance of <see cref="NamesController"/>.
    /// </summary>
    /// <param name="humanNameService">The human name generation service.</param>
    public NamesController(IHumanNameService humanNameService)
    {
        _humanNameService = humanNameService;
    }

    /// <summary>
    /// Generates human names from the SSA and Census datasets.
    /// </summary>
    /// <param name="request">Filter parameters for name generation.</param>
    /// <returns>A list of generated names matching the specified filters.</returns>
    [HttpGet("human")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHumanNames([FromQuery] HumanNameRequest request)
    {
        var response = await _humanNameService.GenerateAsync(request);
        return Ok(response);
    }
}