using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using TagService.Domain.Entities;

namespace TagService.Infrastructure.Data;

public class TagServiceDbContext : DbContext {

    private readonly string _connectionString;
    
    public DbSet<Tag> Tags { get; set; }
    public DbSet<WatchedTag> WatchedTags { get; set; }
    public DbSet<UserTagParticipation> UserTagParticipations { get; set; }
    public DbSet<UserTagParticipationQuestion> UserTagParticipationQuestions { get; set; }

    public TagServiceDbContext (string connectionString) {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring (DbContextOptionsBuilder options) {
        options.UseNpgsql (_connectionString);
        options.EnableSensitiveDataLogging();
        options.UseLoggerFactory (CreateLoggerFactory());
    }
    
    protected override void OnModelCreating (ModelBuilder modelBuilder) {
        modelBuilder.Entity<Tag> ().HasMany (t => t.UserTagParticipations).WithOne (utp => utp.Tag)
           .HasForeignKey (utp => utp.TagId);

        modelBuilder.Entity<Tag> ().HasMany (t => t.WatchedTags).WithOne (wt => wt.Tag).HasForeignKey (wt => wt.TagId);

        modelBuilder.Entity<Tag> ().HasData (
            new Tag (id: 1, name: "csharp", description: "Язык C#"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 2, name: "asp.net-core", description: "ASP.NET Core 9"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 3, name: "entity-framework", description: "Entity Framework Core"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 4, name: "linq", description: "LINQ‑выражения"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 5, name: "sql", description: "SQL‑запросы"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 6, name: "postgresql", description: "PostgreSQL"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 7, name: "docker", description: "Контейнеризация"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 8, name: "rabbitmq", description: "Очереди RabbitMQ"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 9, name: "rest", description: "REST‑API"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc))
            , new Tag (id: 10, name: "microservices", description: "Микросервисная арх‑ра"
                , createdAt: new DateTime (2025, 7, 17, 0, 0, 0, DateTimeKind.Utc)));

    }
    
    private ILoggerFactory CreateLoggerFactory () =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });

}
