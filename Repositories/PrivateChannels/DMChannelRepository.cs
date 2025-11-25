using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories.PrivateChannels;
public class DMChannelRepository(AppDbContext db) : IRepository<DMChannel> {
    private readonly AppDbContext _db = db;

    public async Task<DMChannel> AddAsync(DMChannel entity, bool save = true) {
        _db.DMChannels.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(DMChannel entity, bool save = true) {
        _db.DMChannels.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<DMChannel?> GetAsync(long id) => await _db.DMChannels.FindAsync(id);
    public async Task<IEnumerable<DMChannel>> GetAllAsync() => await _db.DMChannels.ToListAsync();
    public async Task<IEnumerable<DMChannel>> GetForIdAsync(long userId) =>
        await _db.DMChannels.Where(c => c.Members.Contains(userId)).ToListAsync();
    public async Task UpdateAsync(DMChannel entity, bool save = true) {
        _db.DMChannels.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<DMChannel>> FindAsync(Expression<Func<DMChannel, bool>> predicate) =>
        await _db.DMChannels.Where(predicate).ToListAsync();
}