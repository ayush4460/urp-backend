using URP.Domain.Common;

namespace URP.Domain.Entities;

public sealed class User : AuditableEntity<long>
{
    private User() { }

    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public long? LastLoginAt { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    public string FullName => $"{FirstName} {LastName}".Trim();

    public static User Create(
        string username, string email, string passwordHash,
        string firstName, string lastName) =>
        new()
        {
            Username = username.Trim().ToLowerInvariant(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            IsActive = true,
        };

    public void UpdateProfile(string firstName, string lastName, string username)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Username = username.Trim().ToLowerInvariant();
        Touch();
    }

    public void UpdatePasswordHash(string newHash) { PasswordHash = newHash; Touch(); }
    public void SetActive(bool isActive) { IsActive = isActive; Touch(); }
    public void RecordLogin() { LastLoginAt = EpochHelper.NowSeconds(); Touch(); }
}