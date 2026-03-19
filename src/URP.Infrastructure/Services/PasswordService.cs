using Microsoft.AspNetCore.Identity;
using URP.Application.Interfaces;

namespace URP.Infrastructure.Services;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string plainPassword) =>
        _hasher.HashPassword(null!, plainPassword);

    public bool Verify(string plainPassword, string hash) =>
        _hasher.VerifyHashedPassword(null!, hash, plainPassword)
            != PasswordVerificationResult.Failed;
}
