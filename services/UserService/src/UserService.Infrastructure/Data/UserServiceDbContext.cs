using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data;

public class UserServiceDbContext : DbContext {

    public UserServiceDbContext (DbContextOptions<UserServiceDbContext> options) : base(options) {}

    public DbSet<UserInfo> UsersInfos { get; set; } // TODO ПЕРЕИМЕНОВАТЬ В UsersInfo
    public DbSet<UserStatistic> UsersStatistic { get; set; }

    protected override void OnModelCreating (ModelBuilder modelBuilder) {
        modelBuilder.Entity<UserInfo> ().HasKey (x => x.Id);
        modelBuilder.Entity<UserStatistic> ().HasKey (x => x.Id);
    }
}
