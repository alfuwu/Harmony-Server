using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<User> Users { get; set; }
    public DbSet<GuildServer> Servers { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Message> Messages { get; set; }
}