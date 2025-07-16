using AnswerService.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnswerService.Infrastructure;

public class AnswerServiceDbContext : DbContext {

  private readonly string _connectionString;
  
  public DbSet<Answer> Answers { get; set; }
  public DbSet<AnswerChangingHistory>  AnswerChangingHistories { get; set; }

  public AnswerServiceDbContext (string connectionString) {
    _connectionString = connectionString;
  }

  protected override void OnConfiguring (DbContextOptionsBuilder options) {
    options.UseNpgsql (_connectionString);
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory (CreateLoggerFactory());
  }

  protected override void OnModelCreating (ModelBuilder builder) {
    base.OnModelCreating(builder);
    
    builder.ApplyConfigurationsFromAssembly (typeof(AnswerServiceDbContext).Assembly);
  }

  private ILoggerFactory CreateLoggerFactory () =>
    LoggerFactory.Create(builder => { builder.AddConsole(); });

}