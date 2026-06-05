namespace JuniorGolf.Shared.DTOs;

public record MemberDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Status
);
