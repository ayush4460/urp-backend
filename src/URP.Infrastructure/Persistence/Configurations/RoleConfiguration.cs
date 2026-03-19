using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("roles");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        b.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        b.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);
        b.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(e => e.CreatedAt).HasColumnName("created_at");  
        b.Property(e => e.UpdatedAt).HasColumnName("updated_at");  

        b.HasIndex(e => e.Name).IsUnique().HasDatabaseName("uk_roles_name");
    }
}
