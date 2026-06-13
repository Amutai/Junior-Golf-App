using JuniorGolf.Core.Interfaces;
using JuniorGolf.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JuniorGolf.Api.Controllers;

/// <summary>
/// Authentication endpoints.
///
/// Data flow:
///   POST /api/auth/register → AuthController → IAuthService.RegisterAsync → Identity + JWT → AuthResponseDto
///   POST /api/auth/login    → AuthController → IAuthService.LoginAsync    → Identity + JWT → AuthResponseDto
///
/// No [Authorize] attribute — these endpoints are public.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(
            new RegisterRequest(dto.Email, dto.Password, dto.FirstName, dto.LastName));

        if (!result.Success)
            return BadRequest(new AuthResponseDto(false, null, null, result.Error));

        return Created("", new AuthResponseDto(true, result.Token, result.RefreshToken, null));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(new LoginRequest(dto.Email, dto.Password));

        if (!result.Success)
            return Unauthorized(new AuthResponseDto(false, null, null, result.Error));

        return Ok(new AuthResponseDto(true, result.Token, result.RefreshToken, null));
    }
}
