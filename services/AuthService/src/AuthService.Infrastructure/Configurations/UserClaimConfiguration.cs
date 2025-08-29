using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
  public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
  {
    builder.ToTable("user_claims");

    builder.HasKey(uc => uc.Id).HasName("pk_user_claims_id");

    builder.Property(uc => uc.Id).HasColumnName("id");
    builder.Property(uc => uc.UserId).HasColumnName("user_id").IsRequired();
    builder.Property(uc => uc.ClaimType).HasColumnName("claim_type");
    builder.Property(uc => uc.ClaimValue).HasColumnName("claim_value");
  }
}
