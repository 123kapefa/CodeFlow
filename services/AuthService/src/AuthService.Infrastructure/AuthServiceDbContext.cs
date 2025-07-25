using AuthService.Domain.Entities;

using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure;

public class AuthServiceDbContext : IdentityDbContext<UserData, IdentityRole<Guid>, Guid> {

  private readonly string _connectionString;
  
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<PendingPasswordChange> PendingPasswordChanges { get; set; }
  
  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
  public DbSet<OutboxState> OutboxStates => Set<OutboxState>();
  public DbSet<InboxState> InboxStates => Set<InboxState>();

  public AuthServiceDbContext (string connectionString) {
    _connectionString = connectionString;
  }
  
  protected override void OnConfiguring (DbContextOptionsBuilder options) {
    options.UseNpgsql (_connectionString);
    options.EnableSensitiveDataLogging ();
    options.UseLoggerFactory (CreateLoggerFactory ());
  }

  protected override void OnModelCreating (ModelBuilder builder) {
    base.OnModelCreating (builder);

    builder.AddInboxStateEntity();
    builder.AddOutboxMessageEntity();
    builder.AddOutboxStateEntity();

    
    builder.ApplyConfigurationsFromAssembly (typeof(AuthServiceDbContext).Assembly);
  }
  
  private ILoggerFactory CreateLoggerFactory () => 
    LoggerFactory.Create (builder => { builder.AddConsole (); });
}