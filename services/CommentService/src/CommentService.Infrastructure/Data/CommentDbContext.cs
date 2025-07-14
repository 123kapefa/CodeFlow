using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Infrastructure.Data;

public class CommentDbContext : DbContext {   

    public DbSet<Comment> Comments { get; set; }

    public CommentDbContext( DbContextOptions<CommentDbContext> options ) : base(options) {}

    protected override void OnModelCreating( ModelBuilder modelBuilder ) {

        modelBuilder.HasPostgresEnum<TypeTarget>();

        modelBuilder.Entity<Comment>(mb => {
            mb.ToTable("comments");
            mb.Property(x => x.Type).HasColumnType("type_target");  
            mb.HasIndex(x => new { x.Type, x.TargetId, x.CreatedAt}).HasDatabaseName("idx_comments_target");
        });
    }

}
