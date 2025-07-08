using AuthService.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure;

public class AuthServiceDbContext : IdentityDbContext<UserData, IdentityRole<Guid>, Guid> {

  private readonly string _connectionString;

  public AuthServiceDbContext (string connectionString) {
    _connectionString = connectionString;
  }
  
  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnConfiguring (DbContextOptionsBuilder options) {
    options.UseNpgsql (_connectionString);
    options.EnableSensitiveDataLogging ();
    options.UseLoggerFactory (CreateLoggerFactory ());
  }

  protected override void OnModelCreating (ModelBuilder builder) {
    base.OnModelCreating (builder);

    builder.ApplyConfigurationsFromAssembly (typeof(AuthServiceDbContext).Assembly);
  }
  
  private ILoggerFactory CreateLoggerFactory () => 
    LoggerFactory.Create (builder => { builder.AddConsole (); });
}