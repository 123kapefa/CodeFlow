using AnswerService.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnswerService.Infrastructure.Configurations;

public class AnswerChangingHistoryConfiguration : IEntityTypeConfiguration<AnswerChangingHistory>
{
  public void Configure(EntityTypeBuilder<AnswerChangingHistory> builder)
  {
    builder.ToTable("answer_changing_histories");
    builder.HasKey(h => h.Id);

    builder.HasKey(x => x.Id)
     .HasName ("pk_answer_changing_history_id");
    
    builder.Property(h => h.Id)
     .HasColumnName("id")
     .IsRequired();

    builder.Property(h => h.AnswerId)
     .HasColumnName("answer_id")
     .IsRequired();

    builder.Property(h => h.UserId)
     .HasColumnName("user_id")
     .IsRequired();

    builder.Property(h => h.Content)
     .HasColumnName("content")
     .IsRequired()
     .HasColumnType("text");

    builder.Property(h => h.UpdatedAt)
     .HasColumnName("updated_at")
     .IsRequired();

    builder.HasIndex(h => h.AnswerId)
     .HasDatabaseName("ix_answer_history_answer_id");

    builder.HasIndex(h => h.UserId)
     .HasDatabaseName("ix_answer_history_user_id");
    
    builder.HasOne<Answer>()
     .WithMany(a => a.AnswerChangingHistoriesChanges)
     .HasForeignKey(h => h.AnswerId)
     .OnDelete(DeleteBehavior.Cascade);
   
  }
}