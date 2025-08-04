using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;

using Microsoft.Extensions.Logging;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit;

namespace QuestionService.Infrastructure.Data;

public class QuestionServiceDbContext : DbContext {

    private readonly string _connectionString;
    
    public QuestionServiceDbContext (string connectionString) {
        _connectionString = connectionString;
    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionChangingHistory> QuestionChangingHistories { get; set; }
    public DbSet<QuestionTag> QuestionTags { get; set; }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxState> OutboxStates => Set<OutboxState>();
    public DbSet<InboxState> InboxStates => Set<InboxState>();

    protected override void OnConfiguring (DbContextOptionsBuilder options) {
        options.UseNpgsql (_connectionString);
        options.EnableSensitiveDataLogging();
        options.UseLoggerFactory (CreateLoggerFactory());
    }
    
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

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

    }
    
    private ILoggerFactory CreateLoggerFactory () =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });

}
