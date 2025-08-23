using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure.Configurations;

public class ReputationEffectConfiguration : IEntityTypeConfiguration<ReputationEffect> {

  public void Configure (EntityTypeBuilder<ReputationEffect> b) {
    b.ToTable ("reputation_effects");
    b.HasKey (x => x.Id);

    b.Property (x => x.SourceType).HasColumnName ("source_type").HasConversion<string> ().HasMaxLength (16).IsRequired ();
    b.Property (x => x.EffectKind).HasColumnName ("effect_kind").HasMaxLength (32).IsRequired ();
    b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
    b.Property(x => x.SourceId).HasColumnName("source_id").IsRequired();
    b.Property (x => x.Amount).HasColumnName("amount").IsRequired ();
    b.Property (x => x.ParentId).HasColumnName("parent_id").IsRequired ();
    b.Property (x => x.LastVersion).HasColumnName("last_version").IsRequired ();
    b.Property (x => x.LastEventId).HasColumnName("last_event_id").IsRequired ();
    b.Property (x => x.UpdatedAt).HasColumnName("updated_at").IsRequired ();
    b.Property (x => x.SourceService).HasColumnName("source_service").HasMaxLength (64).IsRequired ();
    
    b.HasIndex (x => new { x.UserId, x.SourceId, x.SourceType, x.EffectKind }).IsUnique ();
  }

}