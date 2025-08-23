using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VoteService.Domain.Entities;

namespace VoteService.Infrastructure.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote> {

  public void Configure (EntityTypeBuilder<Vote> b) {
    b.ToTable ("votes");
    b.HasKey (x => x.Id);

    b.Property (x => x.SourceType).HasConversion<string> ().HasMaxLength (16).IsRequired ();
    b.Property (x => x.Kind).HasConversion<string> ().HasMaxLength (8).IsRequired ();
    b.Property (x => x.ChangedAt).IsRequired ();
    
    b.HasIndex (x => new { x.AuthorUserId, x.SourceType, x.SourceId }).IsUnique ();
  }

}