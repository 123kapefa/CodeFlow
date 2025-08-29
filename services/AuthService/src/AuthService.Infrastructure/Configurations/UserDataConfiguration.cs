using AuthService.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static AuthService.Domain.LengthConstants;

namespace AuthService.Infrastructure.Configurations;

public class UserDataConfiguration : IEntityTypeConfiguration<UserData>
{
    public void Configure(EntityTypeBuilder<UserData> builder)
    {
        builder.ToTable("users_data");

        builder.HasKey(u => u.Id).HasName("pk_users_data_id");

        builder.Property (u => u.IsExternal)
           .HasColumnName ("is_external")
           .IsRequired ();

        builder.Property (u => u.Fullname)
           .HasColumnName ("fullname")
           .HasMaxLength (LENGTH20);
        
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasColumnName("username")
            .HasMaxLength(LENGTH256);

        builder.Property(u => u.NormalizedUserName)
            .HasColumnName("normalized_username")
            .HasMaxLength(LENGTH256);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(LENGTH256);

        builder.Property(u => u.NormalizedEmail)
            .HasColumnName("normalized_email")
            .HasMaxLength(LENGTH256);

        builder.Property(u => u.EmailConfirmed)
            .HasColumnName("email_confirmed")
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash");

        builder.Property(u => u.SecurityStamp)
            .HasColumnName("security_stamp");

        builder.Property(u => u.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");

        builder.Property(u => u.PhoneNumber)
            .HasColumnName("phone_number");

        builder.Property(u => u.PhoneNumberConfirmed)
            .HasColumnName("phone_number_confirmed")
            .IsRequired();

        builder.Property(u => u.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled")
            .IsRequired();

        builder.Property(u => u.LockoutEnd)
            .HasColumnName("lockout_end");

        builder.Property(u => u.LockoutEnabled)
            .HasColumnName("lockout_enabled")
            .IsRequired();

        builder.Property(u => u.AccessFailedCount)
            .HasColumnName("access_failed_count")
            .IsRequired();

        builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("ix_users_data_normalized_username").IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("ix_users_data_normalized_email");

        builder.HasIndex(u => u.Email).HasDatabaseName("ix_users_data_email");
    }
}