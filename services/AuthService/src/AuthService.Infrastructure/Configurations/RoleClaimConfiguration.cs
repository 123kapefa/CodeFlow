using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
  public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
  {
    builder.ToTable("role_claims");

    builder.HasKey(rc => rc.Id).HasName("pk_role_claims_id");

    builder.Property(rc => rc.Id).HasColumnName("id");
    builder.Property(rc => rc.RoleId).HasColumnName("role_id");
    builder.Property(rc => rc.ClaimType).HasColumnName("claim_type");
    builder.Property(rc => rc.ClaimValue).HasColumnName("claim_value");
  }
}
