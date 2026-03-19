using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("user_roles");
        b.HasKey(e => new { e.UserId, e.RoleId });
        b.Property(e => e.UserId).HasColumnName("user_id");
        b.Property(e => e.RoleId).HasColumnName("role_id");
        b.Property(e => e.AssignedAt).HasColumnName("assigned_at");
        b.Property(e => e.AssignedBy).HasColumnName("assigned_by");

        b.HasOne(e => e.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(e => e.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}