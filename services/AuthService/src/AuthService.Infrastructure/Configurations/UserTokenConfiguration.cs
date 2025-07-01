using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
  public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
  {
    builder.ToTable("user_tokens");

    builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name }).HasName("pk_user_tokens");

    builder.Property(t => t.UserId).HasColumnName("user_id");
    builder.Property(t => t.LoginProvider).HasColumnName("login_provider").HasMaxLength(128);
    builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(128);
    builder.Property(t => t.Value).HasColumnName("value");
  }
}
