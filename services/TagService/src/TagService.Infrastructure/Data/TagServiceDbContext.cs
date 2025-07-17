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
    }
}
