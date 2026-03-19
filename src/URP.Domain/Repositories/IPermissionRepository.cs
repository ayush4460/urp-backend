using URP.Domain.Entities;

namespace URP.Domain.Repositories;

public interface IPermissionRepository : IRepository<Permission, int>
{
    Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetByGroupAsync(string group, CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> IsAssignedToRoleAsync(int roleId, int permissionId, CancellationToken ct = default);
    Task<RolePermission?> GetRolePermissionAsync(int roleId, int permissionId, CancellationToken ct = default);
    Task AddRolePermissionAsync(RolePermission rp, CancellationToken ct = default);
    void RemoveRolePermission(RolePermission rp);
}
