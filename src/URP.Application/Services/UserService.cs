using AutoMapper;
using Microsoft.Extensions.Logging;
using URP.Application.Common;
using URP.Application.DTOs.Auth;
using URP.Application.DTOs.Users;
using URP.Application.Interfaces;
using URP.Domain.Entities;
using URP.Domain.Exceptions;
using URP.Domain.Repositories;

namespace URP.Application.Services;

public sealed class UserService(
    IUnitOfWork uow,
    IMapper mapper,
    ITokenService tokenService,
    IPasswordService passwordService,
    ILogger<UserService> logger) : IUserService
{
    public async Task<UserResponse> RegisterAsync(CreateUserRequest req, CancellationToken ct)
    {
        if (await uow.Users.ExistsByEmailAsync(req.Email, ct))
            throw new ConflictException($"The email '{req.Email}' is already registered.");

        if (await uow.Users.ExistsByUsernameAsync(req.Username, ct))
            throw new ConflictException($"The username '{req.Username}' is already taken.");

        var hash = passwordService.Hash(req.Password);
        var user = User.Create(req.Username, req.Email, hash, req.FirstName, req.LastName);

        await uow.Users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        var userRole = await uow.Roles.GetByNameAsync("User", ct);
        if (userRole != null)
        {
            await uow.Roles.AddUserRoleAsync(
                UserRole.Create(user.Id, userRole.Id), ct);
            await uow.SaveChangesAsync(ct);
        }

        logger.LogInformation("User registered. Id={UserId} Email={Email}", user.Id, user.Email);
        return await GetByIdAsync(user.Id, ct);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await uow.Users.GetByEmailWithRolesAsync(req.Email, ct)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("This account has been deactivated.");

        if (!passwordService.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        user.RecordLogin();
        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);

        var token = tokenService.GenerateToken(user);

        logger.LogInformation("User logged in. Id={UserId}", user.Id);
        return new LoginResponse
        {
            AccessToken  = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresIn    = token.ExpiresInSeconds,
            User         = mapper.Map<UserResponse>(user),
        };
    }

    public async Task<UserResponse> GetByIdAsync(long id, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdWithRolesAndPermissionsAsync(id, ct)
            ?? throw new NotFoundException("User", id);
        return mapper.Map<UserResponse>(user);
    }

    public async Task<PaginatedResponse<UserResponse>> GetAllAsync(PaginationQuery query, CancellationToken ct)
    {
        var (items, total) = await uow.Users.GetPaginatedAsync(
            query.Page, query.PageSize, query.Search, query.SortBy, query.SortDescending, ct);

        return new PaginatedResponse<UserResponse>
        {
            Items      = mapper.Map<IEnumerable<UserResponse>>(items),
            TotalCount = total,
            Page       = query.Page,
            PageSize   = query.PageSize,
        };
    }

    public async Task<UserResponse> UpdateAsync(long id, UpdateUserRequest req, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("User", id);

        var newFirst    = req.FirstName ?? user.FirstName;
        var newLast     = req.LastName  ?? user.LastName;
        var newUsername = req.Username  ?? user.Username;

        if (req.Username != null && req.Username != user.Username)
            if (await uow.Users.ExistsByUsernameAsync(req.Username, ct, excludeId: id))
                throw new ConflictException($"Username '{req.Username}' is already taken.");

        user.UpdateProfile(newFirst, newLast, newUsername);

        if (req.IsActive.HasValue) user.SetActive(req.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(req.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(req.CurrentPassword))
                throw new BusinessRuleException("Current password is required when changing password.");
            if (!passwordService.Verify(req.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedException("Current password is incorrect.");
            user.UpdatePasswordHash(passwordService.Hash(req.NewPassword));
        }

        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);
        logger.LogInformation("User updated. Id={UserId}", id);
        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("User", id);
        user.SoftDelete();
        user.SetActive(false);
        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);
        logger.LogInformation("User soft-deleted. Id={UserId}", id);
    }
}
