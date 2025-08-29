using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure.Configurations;

file class ReputationSummaryConfiguration : IEntityTypeConfiguration<ReputationSummary> {

  public void Configure (EntityTypeBuilder<ReputationSummary> b) {
    b.ToTable ("reputation_summaries");
    b.HasKey (x => x.UserId);
    
    b.Property (x => x.UserId).HasColumnName ("user_id").IsRequired ();

    b.Property (x => x.Total).HasColumnName ("total").IsRequired ();

    b.HasIndex (x => x.Total);
  }

}