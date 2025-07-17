using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagService.Domain.Entities;

namespace TagService.Infrastructure.Data;

public class TagServiceDbContext : DbContext {

    public DbSet<Tag> Tags { get; set; }
    public DbSet<WatchedTag> WatchedTags { get; set; }
    public DbSet<UserTagParticipation> UserTagParticipations { get; set; }

    public TagServiceDbContext( DbContextOptions<TagServiceDbContext> options ) : base(options) {}

    protected override void OnModelCreating( ModelBuilder modelBuilder ) {
        modelBuilder.Entity<Tag>()
            .HasMany(t => t.UserTagParticipations)
            .WithOne(utp => utp.Tag)
            .HasForeignKey(utp => utp.TagId);

        modelBuilder.Entity<Tag>()
            .HasMany(t => t.WatchedTags)
            .WithOne(wt => wt.Tag)
            .HasForeignKey(wt => wt.TagId);

        modelBuilder.Entity<Tag>().HasData(
        new Tag { Id = 1, Name = "csharp", Description = "Язык C#", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 2, Name = "asp.net-core", Description = "ASP.NET Core 9", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 3, Name = "entity-framework", Description = "Entity Framework Core", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 4, Name = "linq", Description = "LINQ‑выражения", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 5, Name = "sql", Description = "SQL‑запросы", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 6, Name = "postgresql", Description = "PostgreSQL", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 7, Name = "docker", Description = "Контейнеризация", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 8, Name = "rabbitmq", Description = "Очереди RabbitMQ", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 9, Name = "rest", Description = "REST‑API", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) },
        new Tag { Id = 10, Name = "microservices", Description = "Микросервисная арх‑ра", CreatedAt = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc) }
    );
    }
}
