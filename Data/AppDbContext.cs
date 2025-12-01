using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    // user stuff
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<UserRelationship> Relationships { get; set; }

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
    public DbSet<Attachment> Attachments { get; set; }

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
            })
            .HasMany(m => m.Attachments)
            .WithOne(a => a.Message)
            .HasForeignKey(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GuildServer>()
            .HasMany(s => s.Emojis)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<GuildServer>()
            .HasOne(u => u.Settings)
            .WithOne(s => s.Server)
            .HasForeignKey<ServerSettings>(s => s.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ServerSettings>()
            .HasKey(s => s.ServerId);
        modelBuilder.Entity<GuildServer>()
            .HasMany(s => s.Members)
            .WithOne(m => m.Server)
            .HasForeignKey(m => m.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Settings)
            .WithOne(s => s.User)
            .HasForeignKey<UserSettings>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserSettings>()
            .HasKey(s => s.UserId);
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasColumnType("citext"); // make usernames case insensitive
        modelBuilder.Entity<UserRelationship>(entity => {
            entity.HasKey(e => new { e.First, e.Second});

            // Enforce order to maintain symmetry (always store smaller ID first)
            entity.Property(e => e.First).IsRequired();
            entity.Property(e => e.Second).IsRequired();

            entity.ToTable(tb => tb.HasCheckConstraint(
                "CK_UserRelationship_UserIdOrder",
                "[First] < [Second]"
            ));

            // enum stored as byte
            entity.Property(e => e.Relationship).HasConversion<byte>().IsRequired();
        });

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

        // TPT mappings
        modelBuilder.Entity<AbstractChannel>().ToTable("AChans");
        modelBuilder.Entity<Channel>().ToTable("Channels");
        modelBuilder.Entity<DmChannel>().ToTable("DmChannels");
        modelBuilder.Entity<GroupDmChannel>().ToTable("GroupDmChannels");
        modelBuilder.Entity<ThreadChannel>().ToTable("Threads");

        modelBuilder.Entity<Emoji>().ToTable("Emojis");
        modelBuilder.Entity<Member>().ToTable("Members");
        modelBuilder.Entity<Attachment>().ToTable("Attachments");

        // set first id to be 0 instead of 1
        modelBuilder.Entity<Emoji>()
            .Property(e => e.Id)
            .HasIdentityOptions(minValue: 0, startValue: 0);
        modelBuilder.Entity<Role>()
            .Property(u => u.Id)
            .HasIdentityOptions(minValue: 0, startValue: 0);
        modelBuilder.Entity<AbstractChannel>()
            .Property(u => u.Id)
            .HasIdentityOptions(minValue: -1, startValue: -1);
        modelBuilder.Entity<Message>()
            .Property(u => u.Id)
            .HasIdentityOptions(minValue: 0, startValue: 0);
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .HasIdentityOptions(minValue: -1, startValue: -1);
        modelBuilder.Entity<GuildServer>()
            .Property(u => u.Id)
            .HasIdentityOptions(minValue: -1, startValue: -1);
        modelBuilder.Entity<Attachment>()
            .Property(u => u.Id)
            .HasIdentityOptions(minValue: 0, startValue: 0);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        var deletedMessages = ChangeTracker.Entries<Message>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in deletedMessages) {
            var attachments = entry.Entity.Attachments;
            if (attachments != null)
                foreach (var attachment in attachments)
                    if (File.Exists(attachment.FilePath))
                        File.Delete(attachment.FilePath);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}