using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("permissions");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        b.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        b.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);
        b.Property(e => e.Group).HasColumnName("perm_group").HasMaxLength(50).IsRequired();
        b.Property(e => e.CreatedAt).HasColumnName("created_at"); 
        b.Property(e => e.UpdatedAt).HasColumnName("updated_at"); 

        b.HasIndex(e => e.Name).IsUnique().HasDatabaseName("uk_permissions_name");
        b.HasIndex(e => e.Group).HasDatabaseName("idx_permissions_group");
    }
}
