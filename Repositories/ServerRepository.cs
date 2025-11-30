using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories;
public class ServerRepository(AppDbContext db) : IRepository<GuildServer> {
    private readonly AppDbContext _db = db;

    public async Task<GuildServer> AddAsync(GuildServer entity, bool save = true) {
        _db.Servers.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(GuildServer entity, bool save = true) {
        _db.Servers.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<GuildServer?> GetAsync(long id) => await _db.Servers
        .Include(s => s.Members)
        .ThenInclude(m => m.User)
        //.Include(s => s.Roles)
        //.Include(s => s.Emojis)
        .FirstAsync(s => s.Id == id);
    public async Task<IEnumerable<GuildServer>> GetAllAsync() => await _db.Servers.ToListAsync();
    public async Task<IEnumerable<GuildServer>> GetForIdAsync(long userId) =>
        await _db.Servers
            //.Include(s => s.Roles)
            //.Include(s => s.Emojis)
            .Where(s => s.OwnerId == userId || s.Members
                .Select(m => m.UserId)
                .Contains(userId))
            .ToListAsync();
    public async Task UpdateAsync(GuildServer entity, bool save = true) {
        _db.Servers.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<GuildServer>> FindAsync(Expression<Func<GuildServer, bool>> predicate) =>
        await _db.Servers.Where(predicate).ToListAsync();
}