using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<User> Users { get; set; }
    public DbSet<GuildServer> Servers { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<DMChannel> DMChannels { get; set; }
    public DbSet<GroupDMChannel> GroupDMChannels { get; set; }
    public DbSet<ThreadChannel> Threads { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Message>(entity => {
            entity.OwnsMany(m => m.Reactions, r => {
                r.Property(e => e.Reactors);
                r.OwnsOne(e => e.Emoji);
            });
        });

        modelBuilder.Entity<Role>(entity => {
            entity.OwnsMany(r => r.Prerequisites, p => {

            });
        });
    }
}