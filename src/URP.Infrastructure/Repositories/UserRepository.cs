using Microsoft.EntityFrameworkCore;
using URP.Domain.Entities;
using URP.Domain.Repositories;
using URP.Infrastructure.Persistence;

namespace URP.Infrastructure.Repositories;

public sealed class UserRepository(ApplicationDbContext db)
    : BaseRepository<User, long>(db), IUserRepository
{
    private IQueryable<User> WithFullGraph() =>
        _set.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        => await _set.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct)
        => await WithFullGraph().FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<User?> GetByIdWithRolesAndPermissionsAsync(long id, CancellationToken ct)
        => await WithFullGraph().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct, long? excludeId = null)
        => await _set.AnyAsync(u =>
            u.Email == email.ToLowerInvariant() && (excludeId == null || u.Id != excludeId), ct);

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct, long? excludeId = null)
        => await _set.AnyAsync(u =>
            u.Username == username.ToLowerInvariant() && (excludeId == null || u.Id != excludeId), ct);

    public async Task<(IEnumerable<User>, int)> GetPaginatedAsync(
        int page, int pageSize, string? search, string? sortBy, bool sortDescending, CancellationToken ct)
    {
        var query = _set.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(s) ||
                u.Email.ToLower().Contains(s) ||
                u.FirstName.ToLower().Contains(s) ||
                u.LastName.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        query = (sortBy?.ToLower(), sortDescending) switch
        {
            ("email",    true)  => query.OrderByDescending(u => u.Email),
            ("email",    false) => query.OrderBy(u => u.Email),
            ("username", true)  => query.OrderByDescending(u => u.Username),
            ("username", false) => query.OrderBy(u => u.Username),
            (_,          true)  => query.OrderByDescending(u => u.CreatedAt),
            _                   => query.OrderBy(u => u.CreatedAt),
        };

        var items = await query
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId, CancellationToken ct)
        => await _set
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId))
            .ToListAsync(ct);
}
