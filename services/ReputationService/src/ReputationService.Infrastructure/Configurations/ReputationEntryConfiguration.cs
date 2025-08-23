using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReputationEntryConfiguration : IEntityTypeConfiguration<ReputationEntry> {

  public void Configure (EntityTypeBuilder<ReputationEntry> b) {
    b.ToTable ("reputation_entries");
    b.HasKey (x => x.Id);

    b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
    
    b.Property (x => x.ParentId).HasColumnName("parent_id").IsRequired ();
    
    b.Property(x => x.SourceId).HasColumnName("source_id").IsRequired();
    
    b.Property (x => x.SourceType).HasColumnName ("source_type").HasConversion<string> ().HasMaxLength (16).IsRequired ();

    b.Property (x => x.ReasonCode).HasColumnName ("reason_code").HasConversion<string> ().HasMaxLength (32).IsRequired ();

    b.Property (x => x.ReasonDetails).HasColumnName ("reason_details").HasMaxLength (256);

    b.Property (x => x.Delta).HasColumnName ("delta").IsRequired ();
    
    b.Property (x => x.OccurredAt).HasColumnName ("occurred_at").IsRequired ();

    b.Property (x => x.SourceEventId).HasColumnName ("source_event_id").IsRequired ();

    b.Property (x => x.SourceService).HasColumnName ("source_service").HasMaxLength (64).IsRequired ();
    
    b.HasIndex (x => x.UserId);
    b.HasIndex (x => new { x.UserId, x.OccurredAt });
    b.HasIndex (x => new { x.SourceType, x.SourceId });

    b.HasIndex (x => new { x.SourceEventId, x.UserId }).IsUnique ().HasDatabaseName ("ux_rep_entries_sourceevent_user");
  }

}