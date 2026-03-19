using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URP.Domain.Entities;

namespace URP.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        b.Property(e => e.UserId).HasColumnName("user_id");
        b.Property(e => e.Token).HasColumnName("token").HasMaxLength(512).IsRequired();
        b.Property(e => e.ExpiresAt).HasColumnName("expires_at");   // BIGINT
        b.Property(e => e.RevokedAt).HasColumnName("revoked_at");   // BIGINT nullable
        b.Property(e => e.ReplacedBy).HasColumnName("replaced_by").HasMaxLength(512);
        b.Property(e => e.CreatedAt).HasColumnName("created_at");   // BIGINT
        b.Property(e => e.UpdatedAt).HasColumnName("updated_at");   // BIGINT

        b.HasIndex(e => e.Token).IsUnique().HasDatabaseName("uk_refresh_tokens_token");
        b.HasIndex(e => e.UserId).HasDatabaseName("idx_refresh_tokens_user_id");

        b.HasOne(e => e.User).WithMany()
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

        b.Ignore(e => e.IsActive);
        b.Ignore(e => e.IsExpired);
    }
}
