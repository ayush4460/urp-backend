using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        b.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
        b.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        b.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
        b.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        b.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        b.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(e => e.LastLoginAt).HasColumnName("last_login_at"); 
        b.Property(e => e.CreatedAt).HasColumnName("created_at");    
        b.Property(e => e.UpdatedAt).HasColumnName("updated_at");     
        b.Property(e => e.DeletedAt).HasColumnName("deleted_at");      

        b.HasIndex(e => e.Email).IsUnique().HasDatabaseName("uk_users_email");
        b.HasIndex(e => e.Username).IsUnique().HasDatabaseName("uk_users_username");
        b.HasIndex(e => e.IsActive).HasDatabaseName("idx_users_is_active");
        b.HasIndex(e => e.DeletedAt).HasDatabaseName("idx_users_deleted_at");

        b.Ignore(e => e.FullName);
        b.Ignore(e => e.IsDeleted);
    }
}
