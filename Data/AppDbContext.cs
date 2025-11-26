using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    // user stuff
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }

    // server stuff
    public DbSet<GuildServer> Servers { get; set; }
    public DbSet<ServerSettings> ServerSettings { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Emoji> Emojis { get; set; }

    // channel stuff
    public DbSet<AbstractChannel> AbstractChannels { get; set; }
    public DbSet<Channel> ServerChannels { get; set; }
    public DbSet<DmChannel> DmChannels { get; set; }
    public DbSet<GroupDmChannel> GroupDmChannels { get; set; }
    public DbSet<ThreadChannel> Threads { get; set; }

    // message stuff
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // ownership
        modelBuilder.Entity<Role>()
            .HasOne(r => r.Server)
            .WithMany(s => s.Roles)
            .HasForeignKey(r => r.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>()
            .OwnsMany(r => r.Prerequisites, p => p.ToJson());

        modelBuilder.Entity<Channel>()
            .HasOne(c => c.Server)
            .WithMany()
            .HasForeignKey(c => c.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Channel)
            .WithMany()
            .HasForeignKey(m => m.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .OwnsMany(m => m.Reactions, r => {
                r.ToJson(); // store reactions as JSON
                r.OwnsOne(r => r.Emoji);
            });

        modelBuilder.Entity<GuildServer>()
            .HasMany(s => s.Emojis)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne<UserSettings>()
            .WithOne(s => s.User)
            .HasForeignKey<UserSettings>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserSettings>()
            .HasNoKey();

        modelBuilder.Entity<GuildServer>()
            .HasOne<ServerSettings>()
            .WithOne(s => s.Server)
            .HasForeignKey<ServerSettings>(s => s.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ServerSettings>()
            .HasNoKey();

        modelBuilder.Entity<Member>()
            .HasOne(m => m.Server)
            .WithMany(s => s.Members)
            .HasForeignKey(m => m.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Member>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Member>()
            .HasKey(m => new { m.ServerId, m.UserId });

        modelBuilder.Entity<GuildServer>()
            .HasMany(s => s.Members)
            .WithOne(m => m.Server)
            .HasForeignKey(m => m.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        // TPT mappings
        modelBuilder.Entity<AbstractChannel>().ToTable("AChans");
        modelBuilder.Entity<Channel>().ToTable("Channels");
        modelBuilder.Entity<DmChannel>().ToTable("DmChannels");
        modelBuilder.Entity<GroupDmChannel>().ToTable("GroupDmChannels");
        modelBuilder.Entity<ThreadChannel>().ToTable("Threads");

        modelBuilder.Entity<Emoji>().ToTable("Emojis");
        modelBuilder.Entity<Member>().ToTable("Members");

        // set first id to be 0 instead of 1
        modelBuilder.Entity<Emoji>()
            .Property(e => e.Id)
            .UseIdentityColumn(seed: -1, increment: 1);
        modelBuilder.Entity<Role>()
            .Property(u => u.Id)
            .UseIdentityColumn(seed: -1, increment: 1);
        modelBuilder.Entity<AbstractChannel>()
            .Property(u => u.Id)
            .UseIdentityColumn(seed: -1, increment: 1);
        modelBuilder.Entity<Message>()
            .Property(u => u.Id)
            .UseIdentityColumn(seed: -1, increment: 1);
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .UseIdentityColumn(seed: -1, increment: 1);
        modelBuilder.Entity<GuildServer>()
            .Property(u => u.Id)
            .UseIdentityColumn(seed: -1, increment: 1);
    }
}