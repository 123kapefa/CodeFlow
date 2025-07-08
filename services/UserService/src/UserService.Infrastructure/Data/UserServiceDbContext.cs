using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data;

public class UserServiceDbContext : DbContext {

    public UserServiceDbContext (DbContextOptions<UserServiceDbContext> options) : base(options) {}

    public DbSet<UserInfo> UsersInfos { get; set; } // TODO ПЕРЕИМЕНОВАТЬ В UsersInfo
    public DbSet<UserStatistic> UsersStatistic { get; set; }

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
    }

}