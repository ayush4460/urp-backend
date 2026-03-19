using URP.Domain.Repositories;
using URP.Infrastructure.Persistence;

namespace URP.Infrastructure.Repositories;

public sealed class UnitOfWork(
    ApplicationDbContext db,
    IUserRepository users,
    IRoleRepository roles,
    IPermissionRepository permissions) : IUnitOfWork
{
    private readonly ApplicationDbContext _db = db;
    public IUserRepository       Users       { get; } = users;
    public IRoleRepository       Roles       { get; } = roles;
    public IPermissionRepository Permissions { get; } = permissions;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public void Dispose() => _db.Dispose();
}
