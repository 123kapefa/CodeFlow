using AnswerService.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnswerService.Infrastructure.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer> {

  public void Configure (EntityTypeBuilder<Answer> builder) {
    
    builder.ToTable("answers");
    
    builder.HasKey(x => x.Id)
     .HasName ("pk_answer_id");
    
    builder.Property(x => x.Id)
     .HasColumnName("id")
     .IsRequired ();
    
    builder.Property(a => a.QuestionId)
     .HasColumnName("question_id")
     .IsRequired();

    builder.Property(a => a.UserId)
     .HasColumnName("user_id")
     .IsRequired();

    builder.Property(a => a.UserEditorId)
     .HasColumnName("user_editor_id");

    builder.Property(a => a.Content)
     .HasColumnName("content")
     .IsRequired()
     .HasColumnType("text");

    builder.Property(a => a.CreatedAt)
     .HasColumnName("created_at")
     .IsRequired();

    builder.Property(a => a.UpdatedAt)
     .HasColumnName("updated_at");

    builder.Property(a => a.IsAccepted)
     .HasColumnName("is_accepted")
     .IsRequired();

    builder.Property(a => a.Upvotes)
     .HasColumnName("upvotes")
     .IsRequired();

    builder.Property(a => a.Downvotes)
     .HasColumnName("downvotes")
     .IsRequired();

    builder.HasIndex(a => a.QuestionId)
     .HasDatabaseName("ix_answers_question_id");
    builder.HasIndex(a => a.UserId)
     .HasDatabaseName("ix_answers_user_id");
    builder.HasIndex(a => a.IsAccepted)
     .HasDatabaseName("ix_answers_is_accepted");
    
    builder.HasMany(a => a.AnswerChangingHistoriesChanges)
     .WithOne()
     .HasForeignKey(h => h.AnswerId)
     .OnDelete(DeleteBehavior.Cascade);
    
  }

}