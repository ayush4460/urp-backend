using URP.Domain.Entities;

namespace URP.Domain.Repositories;

public interface IUserRepository : IRepository<User, long>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdWithRolesAndPermissionsAsync(long id, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default, long? excludeId = null);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default, long? excludeId = null);
    Task<(IEnumerable<User> Items, int TotalCount)> GetPaginatedAsync(
        int page, int pageSize, string? search, string? sortBy, bool sortDescending,
        CancellationToken ct = default);
    Task<IEnumerable<User>> GetByRoleIdAsync(int roleId, CancellationToken ct = default);
}
