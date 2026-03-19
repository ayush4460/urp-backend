using AutoMapper;
using Microsoft.Extensions.Logging;
using URP.Application.DTOs.Roles;
using URP.Application.DTOs.Users;
using URP.Application.Interfaces;
using URP.Domain.Entities;
using URP.Domain.Exceptions;
using URP.Domain.Repositories;

namespace URP.Application.Services;

public sealed class RoleService(
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<RoleService> logger) : IRoleService
{
    public async Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken ct)
        => mapper.Map<IEnumerable<RoleResponse>>(await uow.Roles.GetAllWithPermissionsAsync(ct));

    public async Task<RoleResponse> GetByIdAsync(int id, CancellationToken ct)
    {
        var role = await uow.Roles.GetByIdWithPermissionsAsync(id, ct)
            ?? throw new NotFoundException("Role", id);
        return mapper.Map<RoleResponse>(role);
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest req, CancellationToken ct)
    {
        if (await uow.Roles.ExistsByNameAsync(req.Name, ct))
            throw new ConflictException($"Role '{req.Name}' already exists.");

        var role = Role.Create(req.Name, req.Description);
        await uow.Roles.AddAsync(role, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Role created. Name={RoleName}", role.Name);
        return await GetByIdAsync(role.Id, ct);
    }

    public async Task<RoleResponse> UpdateAsync(int id, CreateRoleRequest req, CancellationToken ct)
    {
        var role = await uow.Roles.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Role", id);

        if (!string.Equals(req.Name, role.Name, StringComparison.OrdinalIgnoreCase)
            && await uow.Roles.ExistsByNameAsync(req.Name, ct))
            throw new ConflictException($"Role '{req.Name}' already exists.");

        role.Update(req.Name, req.Description);
        uow.Roles.Update(role);
        await uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var role = await uow.Roles.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Role", id);
        uow.Roles.Remove(role);
        await uow.SaveChangesAsync(ct);
        logger.LogInformation("Role deleted. Id={RoleId}", id);
    }

    public async Task AssignToUserAsync(AssignRoleRequest req, CancellationToken ct)
    {
        _ = await uow.Users.GetByIdAsync(req.UserId, ct)
            ?? throw new NotFoundException("User", req.UserId);
        var role = await uow.Roles.GetByIdAsync(req.RoleId, ct)
            ?? throw new NotFoundException("Role", req.RoleId);

        if (await uow.Roles.IsRoleAssignedToUserAsync(req.UserId, req.RoleId, ct))
            throw new ConflictException($"User already has role '{role.Name}'.");

        await uow.Roles.AddUserRoleAsync(UserRole.Create(req.UserId, req.RoleId), ct);
        await uow.SaveChangesAsync(ct);
        logger.LogInformation("Role '{Role}' assigned to user {UserId}", role.Name, req.UserId);
    }

    public async Task RemoveFromUserAsync(RemoveRoleRequest req, CancellationToken ct)
    {
        var ur = await uow.Roles.GetUserRoleAsync(req.UserId, req.RoleId, ct)
            ?? throw new NotFoundException("Role assignment not found for this user.");
        uow.Roles.RemoveUserRole(ur);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersInRoleAsync(int roleId, CancellationToken ct)
    {
        _ = await uow.Roles.GetByIdAsync(roleId, ct)
            ?? throw new NotFoundException("Role", roleId);
        return mapper.Map<IEnumerable<UserResponse>>(await uow.Users.GetByRoleIdAsync(roleId, ct));
    }
}
