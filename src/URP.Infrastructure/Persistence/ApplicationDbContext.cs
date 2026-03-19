using Microsoft.EntityFrameworkCore;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User>           Users           => Set<User>();
    public DbSet<Role>           Roles           => Set<Role>();
    public DbSet<Permission>     Permissions     => Set<Permission>();
    public DbSet<UserRole>       UserRoles       => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken>   RefreshTokens   => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
       
        mb.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        mb.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);
    }
}
