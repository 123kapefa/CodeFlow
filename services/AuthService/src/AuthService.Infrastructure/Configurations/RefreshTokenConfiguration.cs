using AuthService.Domain;
using AuthService.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken> {

  public void Configure (EntityTypeBuilder<RefreshToken> builder) {
    builder.ToTable("refresh_tokens");
    
    builder.HasKey(rt => rt.Id);
    
    builder.Property(rt => rt.Token)
     .IsRequired()
     .HasMaxLength(LengthConstants.LENGTH200);

    builder.Property(rt => rt.ExpiresAt)
     .IsRequired();

    builder.Property(rt => rt.RevokedAt)
     .IsRequired(false);

    builder.Property(rt => rt.UserId)
     .IsRequired();
    
    builder.HasIndex(rt => rt.Token)
     .IsUnique();
    
    builder
     .HasOne(rt => rt.User)
     .WithMany(u => u.RefreshTokens)
     .HasForeignKey(rt => rt.UserId)
     .OnDelete(DeleteBehavior.Cascade);

   
    builder.HasIndex(rt => rt.UserId);
  }

}