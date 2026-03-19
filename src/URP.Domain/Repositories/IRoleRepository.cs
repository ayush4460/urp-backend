using URP.Domain.Entities;

namespace URP.Domain.Repositories;

public interface IRoleRepository : IRepository<Role, int>
{
    Task<IEnumerable<Role>> GetAllWithPermissionsAsync(CancellationToken ct = default);
    Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> IsRoleAssignedToUserAsync(long userId, int roleId, CancellationToken ct = default);
    Task<UserRole?> GetUserRoleAsync(long userId, int roleId, CancellationToken ct = default);
    Task AddUserRoleAsync(UserRole userRole, CancellationToken ct = default);
    void RemoveUserRole(UserRole userRole);
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
}
