using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Infrastructure.Data;

public class QuestionServiceDbContext : DbContext {

    public QuestionServiceDbContext( DbContextOptions<QuestionServiceDbContext> options ) : base(options) { }

    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionChangingHistory> QuestionChangingHistories { get; set; }
    public DbSet<QuestionTag> QuestionTags { get; set; }

    protected override void OnModelCreating( ModelBuilder modelBuilder ){
        modelBuilder.Entity<Question>()
            .HasMany(q => q.QuestionChangingHistories)
            .WithOne(h => h.Question)
            .HasForeignKey(h => h.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasMany(q => q.QuestionTags)
            .WithOne(qt => qt.Question)
            .HasForeignKey(qt => qt.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
