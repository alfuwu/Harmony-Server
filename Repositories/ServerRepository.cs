using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories;
public class ServerRepository(AppDbContext db) : IRepository<GuildServer> {
    private readonly AppDbContext _db = db;

    public async Task<GuildServer> AddAsync(GuildServer entity) {
        _db.Servers.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(GuildServer entity) {
        _db.Servers.Remove(entity);
        await _db.SaveChangesAsync();
    }
    public async Task<GuildServer?> GetAsync(long id) => await _db.Servers.FindAsync(id);
    public async Task<IEnumerable<GuildServer>> GetAllAsync() => await _db.Servers.ToListAsync();
    public async Task<IEnumerable<GuildServer>> GetForIdAsync(long userId) =>
        await _db.Servers.Where(s => s.OwnerId == userId || s.Members.Contains(userId)).ToListAsync();
    public async Task UpdateAsync(GuildServer entity) {
        _db.Servers.Update(entity);
        await _db.SaveChangesAsync();
    }
    public async Task<IEnumerable<GuildServer>> FindAsync(Expression<Func<GuildServer, bool>> predicate) =>
        await _db.Servers.Where(predicate).ToListAsync();
}