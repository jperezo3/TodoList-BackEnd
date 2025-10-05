using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Interfaces.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint to authenticate users
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token if authentication is successful</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation("Login attempt for user: {Email}", request.Email);

        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login failed for user: {Email}", request.Email);
            return Unauthorized(new { message = result.ErrorMessage });
        }

        _logger.LogInformation("Login successful for user: {Email}", request.Email);
        return Ok(result.Data);
    }
}