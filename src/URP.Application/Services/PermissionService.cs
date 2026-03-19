using AutoMapper;
using Microsoft.Extensions.Logging;
using URP.Application.DTOs.Permissions;
using URP.Application.Interfaces;
using URP.Domain.Entities;
using URP.Domain.Exceptions;
using URP.Domain.Repositories;

namespace URP.Application.Services;

public sealed class PermissionService(
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<PermissionService> logger) : IPermissionService
{
    public async Task<IEnumerable<PermissionResponse>> GetAllAsync(string? group, CancellationToken ct)
    {
        var perms = string.IsNullOrWhiteSpace(group)
            ? await uow.Permissions.GetAllAsync(ct)
            : await uow.Permissions.GetByGroupAsync(group, ct);
        return mapper.Map<IEnumerable<PermissionResponse>>(perms);
    }

    public async Task<PermissionResponse> GetByIdAsync(int id, CancellationToken ct)
    {
        var p = await uow.Permissions.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Permission", id);
        return mapper.Map<PermissionResponse>(p);
    }

    public async Task<PermissionResponse> CreateAsync(CreatePermissionRequest req, CancellationToken ct)
    {
        if (await uow.Permissions.ExistsByNameAsync(req.Name, ct))
            throw new ConflictException($"Permission '{req.Name}' already exists.");

        var p = Permission.Create(req.Name, req.Group, req.Description);
        await uow.Permissions.AddAsync(p, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Permission created. Name={Name}", p.Name);
        return mapper.Map<PermissionResponse>(p);
    }

    public async Task AssignToRoleAsync(AssignPermissionRequest req, CancellationToken ct)
    {
        _ = await uow.Roles.GetByIdAsync(req.RoleId, ct)
            ?? throw new NotFoundException("Role", req.RoleId);
        _ = await uow.Permissions.GetByIdAsync(req.PermissionId, ct)
            ?? throw new NotFoundException("Permission", req.PermissionId);

        if (await uow.Permissions.IsAssignedToRoleAsync(req.RoleId, req.PermissionId, ct))
            throw new ConflictException("Permission is already assigned to this role.");

        await uow.Permissions.AddRolePermissionAsync(
            RolePermission.Create(req.RoleId, req.PermissionId), ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task RemoveFromRoleAsync(AssignPermissionRequest req, CancellationToken ct)
    {
        var rp = await uow.Permissions.GetRolePermissionAsync(req.RoleId, req.PermissionId, ct)
            ?? throw new NotFoundException("Permission assignment not found for this role.");
        uow.Permissions.RemoveRolePermission(rp);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<PermissionResponse>> GetByRoleAsync(int roleId, CancellationToken ct)
    {
        _ = await uow.Roles.GetByIdAsync(roleId, ct)
            ?? throw new NotFoundException("Role", roleId);
        return mapper.Map<IEnumerable<PermissionResponse>>(
            await uow.Permissions.GetByRoleIdAsync(roleId, ct));
    }
}
