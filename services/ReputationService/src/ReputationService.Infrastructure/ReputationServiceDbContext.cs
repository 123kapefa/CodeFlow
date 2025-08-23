using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReputationService.Domain.Entities;

namespace ReputationService.Infrastructure;

public class ReputationServiceDbContext : DbContext {

  private readonly string _connectionString;

  public DbSet<ReputationEffect> ReputationEffects => Set<ReputationEffect>();
  public DbSet<ReputationEntry> ReputationEntries => Set<ReputationEntry>();
  public DbSet<ReputationSummary> ReputationSummaries => Set<ReputationSummary>();
  
  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
  public DbSet<OutboxState> OutboxStates => Set<OutboxState>();
  public DbSet<InboxState> InboxStates => Set<InboxState>();

  public ReputationServiceDbContext (string connectionString) {
    _connectionString = connectionString;
  }
  
  protected override void OnConfiguring (DbContextOptionsBuilder options) {
    options.UseNpgsql (_connectionString);
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory (CreateLoggerFactory());       
  }
  
  protected override void OnModelCreating (ModelBuilder builder) {
    base.OnModelCreating(builder);
    
    builder.ApplyConfigurationsFromAssembly (typeof(ReputationServiceDbContext).Assembly);

    builder.AddInboxStateEntity();
    builder.AddOutboxMessageEntity();
    builder.AddOutboxStateEntity();
  }
  
  private ILoggerFactory CreateLoggerFactory () =>
    LoggerFactory.Create(builder => { builder.AddConsole(); });
  
}