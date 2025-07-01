using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
  public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
  {
    builder.ToTable("roles");

    builder.HasKey(r => r.Id).HasName("pk_roles_id");

    builder.Property(r => r.Id)
     .HasColumnName("id")
     .IsRequired();

    builder.Property(r => r.Name)
     .HasColumnName("name")
     .HasMaxLength(256);

    builder.Property(r => r.NormalizedName)
     .HasColumnName("normalized_name")
     .HasMaxLength(256);

    builder.Property(r => r.ConcurrencyStamp)
     .HasColumnName("concurrency_stamp");

    // Индекс на NormalizedName — как в стандартной реализации
    builder.HasIndex(r => r.NormalizedName)
     .HasDatabaseName("ix_roles_normalized_name")
     .IsUnique();
  }
}