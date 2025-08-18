using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure.Configurations;

public class ReputationEffectConfiguration : IEntityTypeConfiguration<ReputationEffect> {

  public void Configure (EntityTypeBuilder<ReputationEffect> b) {
    b.ToTable ("reputation_effects");
    b.HasKey (x => x.Id);

    b.Property (x => x.SourceType).HasConversion<string> ().HasMaxLength (16).IsRequired ();
    b.Property (x => x.EffectKind).HasMaxLength (32).IsRequired ();

    b.HasIndex (x => new { x.UserId, x.SourceId, x.SourceType, x.EffectKind }).IsUnique ();

    b.Property (x => x.Amount).IsRequired ();
    b.Property (x => x.LastVersion).IsRequired ();
    b.Property (x => x.LastEventId).IsRequired ();
    b.Property (x => x.UpdatedAt).IsRequired ();
    b.Property (x => x.SourceService).HasMaxLength (64).IsRequired ();
    b.Property (x => x.CorrelationId).HasMaxLength (64);
  }

}