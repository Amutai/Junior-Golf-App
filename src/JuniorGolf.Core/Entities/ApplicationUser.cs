using Microsoft.AspNetCore.Identity;

namespace JuniorGolf.Core.Entities;

/// <summary>
/// Application user extending ASP.NET Core Identity.
/// Links authentication (Identity) to domain (Member).
///
/// Relationship: ApplicationUser 1:1 Member (via MemberId)
/// Identity handles: email, password hash, lockout, 2FA
/// We add: name fields and link to member profile
/// </summary>
public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Guid? MemberId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
