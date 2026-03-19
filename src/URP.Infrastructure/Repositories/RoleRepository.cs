using Microsoft.EntityFrameworkCore;
using URP.Domain.Entities;
using URP.Domain.Repositories;
using URP.Infrastructure.Persistence;

namespace URP.Infrastructure.Repositories;

public sealed class RoleRepository(ApplicationDbContext db)
    : BaseRepository<Role, int>(db), IRoleRepository
{
    public async Task<IEnumerable<Role>> GetAllWithPermissionsAsync(CancellationToken ct)
        => await _set
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.Name).ToListAsync(ct);

    public async Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken ct)
        => await _set
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        => await _set.AnyAsync(r => r.Name == name, ct);

    public async Task<bool> IsRoleAssignedToUserAsync(long userId, int roleId, CancellationToken ct)
        => await _db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);

    public async Task<UserRole?> GetUserRoleAsync(long userId, int roleId, CancellationToken ct)
        => await _db.UserRoles.FirstOrDefaultAsync(
            ur => ur.UserId == userId && ur.RoleId == roleId, ct);

    public async Task AddUserRoleAsync(UserRole ur, CancellationToken ct)
        => await _db.UserRoles.AddAsync(ur, ct);

    public void RemoveUserRole(UserRole ur) => _db.UserRoles.Remove(ur);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct)
       => await _set.FirstOrDefaultAsync(r => r.Name == name, ct);
}
