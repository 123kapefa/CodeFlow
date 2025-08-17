using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReputationEntryConfiguration : IEntityTypeConfiguration<ReputationEntry> {

  public void Configure (EntityTypeBuilder<ReputationEntry> b) {
    b.ToTable ("reputation_entries");
    b.HasKey (x => x.Id);

    b.Property (x => x.ReasonCode).IsRequired ().HasMaxLength (64);

    b.Property (x => x.ReasonDetails).HasMaxLength (256);

    b.Property (x => x.Delta).IsRequired ();

    b.Property (x => x.OccurredAt).IsRequired ();

    b.HasIndex (x => x.UserId);
    b.HasIndex (x => new { x.UserId, x.OccurredAt });
    b.HasIndex (x => new { x.SourceType, x.SourceId });
  }

}