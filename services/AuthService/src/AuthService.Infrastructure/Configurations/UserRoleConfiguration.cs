using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
  public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
  {
    builder.ToTable("user_roles");

    builder.HasKey(ur => new { ur.UserId, ur.RoleId }).HasName("pk_user_roles");

    builder.Property(ur => ur.UserId).HasColumnName("user_id");
    builder.Property(ur => ur.RoleId).HasColumnName("role_id");
  }
}
