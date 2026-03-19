using Microsoft.EntityFrameworkCore;
using URP.Domain.Entities;
using URP.Domain.Repositories;
using URP.Infrastructure.Persistence;

namespace URP.Infrastructure.Repositories;

public sealed class PermissionRepository(ApplicationDbContext db)
    : BaseRepository<Permission, int>(db), IPermissionRepository
{
    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct)
        => await _set.OrderBy(p => p.Group).ThenBy(p => p.Name).ToListAsync(ct);

    public async Task<IEnumerable<Permission>> GetByGroupAsync(string group, CancellationToken ct)
        => await _set.Where(p => p.Group == group).OrderBy(p => p.Name).ToListAsync(ct);

    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId, CancellationToken ct)
        => await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .OrderBy(p => p.Name).ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        => await _set.AnyAsync(p => p.Name == name, ct);

    public async Task<bool> IsAssignedToRoleAsync(int roleId, int permId, CancellationToken ct)
        => await _db.RolePermissions.AnyAsync(
            rp => rp.RoleId == roleId && rp.PermissionId == permId, ct);

    public async Task<RolePermission?> GetRolePermissionAsync(int roleId, int permId, CancellationToken ct)
        => await _db.RolePermissions.FirstOrDefaultAsync(
            rp => rp.RoleId == roleId && rp.PermissionId == permId, ct);

    public async Task AddRolePermissionAsync(RolePermission rp, CancellationToken ct)
        => await _db.RolePermissions.AddAsync(rp, ct);

    public void RemoveRolePermission(RolePermission rp) => _db.RolePermissions.Remove(rp);
}
