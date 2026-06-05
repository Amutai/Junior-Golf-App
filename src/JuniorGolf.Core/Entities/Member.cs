namespace JuniorGolf.Core.Entities;

public class Member : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
    public string? HandicapIndex { get; set; }
    public string? ClubAffiliation { get; set; }
}

public enum MembershipStatus
{
    Pending,
    Active,
    Expired,
    Suspended
}
