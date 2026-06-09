using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JuniorGolf.Core.Entities;
using JuniorGolf.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JuniorGolf.Infrastructure.Identity;

/// <summary>
/// Implements IAuthService using ASP.NET Core Identity + JWT.
///
/// Data flow:
///   Register: Input → validate → create user (Identity) → assign role → generate JWT → return
///   Login:    Input → find user → verify password → generate JWT → return
///   Refresh:  Input → validate refresh token → generate new JWT → return
///
/// Dependencies:
///   UserManager<ApplicationUser> — creates/finds users, manages passwords
///   RoleManager<IdentityRole> — manages roles
///   JwtSettings — token configuration (secret, expiry, issuer)
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return new AuthResult(false, Error: "Email already registered");

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return new AuthResult(false, Error: string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Member");

        var token = GenerateJwtToken(user, ["Member"]);
        var refreshToken = GenerateRefreshToken();

        return new AuthResult(true, token, refreshToken);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return new AuthResult(false, Error: "Invalid credentials");

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
            return new AuthResult(false, Error: "Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        return new AuthResult(true, token, refreshToken);
    }

    public Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        // TODO: Validate stored refresh token and issue new JWT
        // Will be fully implemented with Redis token storage in Issue #7
        return Task.FromResult(new AuthResult(false, Error: "Not implemented yet"));
    }

    private string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new("firstName", user.FirstName),
            new("lastName", user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
