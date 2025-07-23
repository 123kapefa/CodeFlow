using AuthService.Domain;
using AuthService.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class PendingPasswordChangeConfiguration : IEntityTypeConfiguration<PendingPasswordChange>
{
  public void Configure(EntityTypeBuilder<PendingPasswordChange> builder)
  {
    builder.ToTable("pending_password_changes");

    builder.HasKey(p => p.Id);
        
    builder.Property(p => p.Id)
     .HasColumnName("id")
     .IsRequired();

    builder.Property(p => p.UserId)
     .HasColumnName("user_id")
     .IsRequired();

    builder.Property(p => p.NewPassword)
     .HasColumnName("new_password")
     .IsRequired()
     .HasMaxLength(LengthConstants.LENGTH256);

    builder.Property(p => p.Token)
     .HasColumnName("token")
     .IsRequired()
     .HasMaxLength(LengthConstants.LENGTH256);

    builder.Property(p => p.CreatedAt)
     .HasColumnName("created_at")
     .IsRequired();

    builder.HasIndex(p => p.Token).IsUnique();
  }
}