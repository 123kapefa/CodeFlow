using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommentService.Infrastructure.Data;

public class CommentServiceDbContext : DbContext {   

    private readonly string _connectionString;
    
    public DbSet<Comment> Comments { get; set; }

    public CommentServiceDbContext( string connectionString ) {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring (DbContextOptionsBuilder options) {
        //options.UseNpgsql (_connectionString);

        options.UseNpgsql( _connectionString,o => o.MapEnum<TypeTarget>("type_target"));

        options.EnableSensitiveDataLogging();
        options.UseLoggerFactory(CreateLoggerFactory());
        options.EnableSensitiveDataLogging();
        options.UseLoggerFactory (CreateLoggerFactory());
    }
    
    protected override void OnModelCreating( ModelBuilder modelBuilder ) {

        modelBuilder.HasPostgresEnum<TypeTarget>();

        modelBuilder.Entity<Comment>(mb => {
            mb.ToTable("comments");
            mb.Property(x => x.Type).HasColumnType("type_target");  
            mb.HasIndex(x => new { x.Type, x.TargetId, x.CreatedAt}).HasDatabaseName("idx_comments_target");
        });
    }
    
    private ILoggerFactory CreateLoggerFactory () =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });


}
