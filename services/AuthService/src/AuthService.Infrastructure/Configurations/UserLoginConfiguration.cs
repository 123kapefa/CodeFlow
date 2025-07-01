using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class UserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
  public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
  {
    builder.ToTable("user_logins");

    builder.HasKey(l => new { l.LoginProvider, l.ProviderKey }).HasName("pk_user_logins");

    builder.Property(l => l.LoginProvider).HasColumnName("login_provider").HasMaxLength(128);
    builder.Property(l => l.ProviderKey).HasColumnName("provider_key").HasMaxLength(128);
    builder.Property(l => l.ProviderDisplayName).HasColumnName("provider_display_name");
    builder.Property(l => l.UserId).HasColumnName("user_id");
  }
}
