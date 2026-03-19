using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using URP.Application.Interfaces;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext db, IPasswordService passwordService, ILogger logger)
    {
        await SeedRolesAsync(db, logger);
        await SeedPermissionsAsync(db, logger);
        await SeedRolePermissionsAsync(db, logger);
        await SeedDefaultAdminAsync(db, passwordService, logger);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Roles.AnyAsync()) return;
        await db.Roles.AddRangeAsync(
            Role.Create("SuperAdmin", "Full system access"),
            Role.Create("Admin",      "Manages users and roles"),
            Role.Create("Manager",    "Read-only access"),
            Role.Create("User",       "Basic authenticated access"));
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded 4 roles.");
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Permissions.AnyAsync()) return;
        await db.Permissions.AddRangeAsync(
            Permission.Create("users:read",         "Users",       "View user list and profiles"),
            Permission.Create("users:create",        "Users",       "Register new users"),
            Permission.Create("users:update",        "Users",       "Update user details"),
            Permission.Create("users:delete",        "Users",       "Soft-delete users"),
            Permission.Create("roles:read",          "Roles",       "View roles and permissions"),
            Permission.Create("roles:create",        "Roles",       "Create new roles"),
            Permission.Create("roles:update",        "Roles",       "Update role details"),
            Permission.Create("roles:delete",        "Roles",       "Delete roles"),
            Permission.Create("roles:assign",        "Roles",       "Assign/remove roles from users"),
            Permission.Create("permissions:read",    "Permissions", "View permissions"),
            Permission.Create("permissions:create",  "Permissions", "Create new permissions"),
            Permission.Create("permissions:assign",  "Permissions", "Assign/remove permissions from roles"));
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded 12 permissions.");
    }

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.RolePermissions.AnyAsync()) return;

        var roles = await db.Roles.ToListAsync();
        var perms = await db.Permissions.ToListAsync();

        int RoleId(string name) => roles.First(r => r.Name == name).Id;
        int PermId(string name) => perms.First(p => p.Name == name).Id;

        var mappings = new List<RolePermission>();

        // SuperAdmin → ALL
        mappings.AddRange(perms.Select(p => RolePermission.Create(RoleId("SuperAdmin"), p.Id)));

        // Admin → user management + role reading/assigning
        foreach (var n in new[] { "users:read","users:create","users:update","users:delete","roles:read","roles:assign","permissions:read" })
            mappings.Add(RolePermission.Create(RoleId("Admin"), PermId(n)));

        // Manager → read-only
        foreach (var n in new[] { "users:read","roles:read","permissions:read" })
            mappings.Add(RolePermission.Create(RoleId("Manager"), PermId(n)));

        await db.RolePermissions.AddRangeAsync(mappings);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded role-permission mappings.");
    }

    private static async Task SeedDefaultAdminAsync(
        ApplicationDbContext db, IPasswordService passwordService, ILogger logger)
    {
        if (await db.Users.IgnoreQueryFilters().AnyAsync()) return;

        var superAdminRole = await db.Roles.FirstAsync(r => r.Name == "SuperAdmin");
        var admin = User.Create(
            "superadmin", "superadmin@urp.local",
            passwordService.Hash("Admin@123"),
            "Super", "Admin");

        await db.Users.AddAsync(admin);
        await db.SaveChangesAsync();
        await db.UserRoles.AddAsync(UserRole.Create(admin.Id, superAdminRole.Id));
        await db.SaveChangesAsync();

        logger.LogInformation("Default SuperAdmin: superadmin@urp.local / Admin@123");
    }
}
