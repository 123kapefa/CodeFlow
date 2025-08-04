using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data;

public class UserServiceDbContext : DbContext {

    private readonly string _connectionString;
    
    public DbSet<UserInfo> UsersInfos { get; set; } // TODO ПЕРЕИМЕНОВАТЬ В UsersInfo
    public DbSet<UserStatistic> UsersStatistic { get; set; }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxState> OutboxStates => Set<OutboxState>();
    public DbSet<InboxState> InboxStates => Set<InboxState>();

    public UserServiceDbContext (string connectionString) {
      _connectionString = connectionString;
    }

    protected override void OnConfiguring (DbContextOptionsBuilder options) {
      options.UseNpgsql (_connectionString);
      options.EnableSensitiveDataLogging();
      options.UseLoggerFactory (CreateLoggerFactory());
    }
    
    protected override void OnModelCreating (ModelBuilder modelBuilder) {

        modelBuilder.Entity<UserInfo>().HasOne(u => u.UserStatistic);       

        modelBuilder.Entity<UserStatistic> ().HasKey (x => x.Id);

        modelBuilder.Entity<UserInfo>()
                   .HasOne(u => u.UserStatistic)        
                   .WithOne()                           
                   .HasForeignKey<UserInfo>(u => u.UserStatisticId) 
                   .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserStatistic>()
                   .Property(s => s.UserId)
                   .ValueGeneratedNever();

        modelBuilder.Entity<UserStatistic>()
                    .HasIndex(s => s.UserId)
                    .IsUnique();

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
    
    private ILoggerFactory CreateLoggerFactory () =>
      LoggerFactory.Create(builder => { builder.AddConsole(); });


}
