namespace JuniorGolf.Shared.DTOs;

public record RegisterDto(string Email, string Password, string FirstName, string LastName);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(bool Success, string? Token, string? RefreshToken, string? Error);
