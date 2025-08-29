using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using VoteService.Domain.Entities;

namespace VoteService.Infrastructure;

public class VoteServiceDbContext : DbContext {

  private readonly string _connectionString;
  
  public DbSet<Vote> Votes { get; set; }

  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
  public DbSet<OutboxState> OutboxStates => Set<OutboxState>();
  public DbSet<InboxState> InboxStates => Set<InboxState>();

  public VoteServiceDbContext (string connectionString) {
    _connectionString = connectionString;
  }
  
  protected override void OnConfiguring (DbContextOptionsBuilder options) {
    options.UseNpgsql (_connectionString);
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory (CreateLoggerFactory());       
  }

  protected override void OnModelCreating (ModelBuilder builder) {
    base.OnModelCreating(builder);
    
    builder.ApplyConfigurationsFromAssembly (typeof(VoteServiceDbContext).Assembly);

    builder.AddInboxStateEntity();
    builder.AddOutboxMessageEntity();
    builder.AddOutboxStateEntity();
  }

  private ILoggerFactory CreateLoggerFactory () =>
    LoggerFactory.Create(builder => { builder.AddConsole(); });

}