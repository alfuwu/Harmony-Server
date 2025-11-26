using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories.PrivateChannels;
public class DmChannelRepository(AppDbContext db) : IRepository<DmChannel> {
    private readonly AppDbContext _db = db;

    public async Task<DmChannel> AddAsync(DmChannel entity, bool save = true) {
        _db.DmChannels.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(DmChannel entity, bool save = true) {
        _db.DmChannels.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<DmChannel?> GetAsync(long id) => await _db.DmChannels.FindAsync(id);
    public async Task<IEnumerable<DmChannel>> GetAllAsync() => await _db.DmChannels.ToListAsync();
    public async Task<IEnumerable<DmChannel>> GetForIdAsync(long userId) =>
        await _db.DmChannels.Where(c => c.Members.Contains(userId) && !c.IsDeleted).ToListAsync();
    public async Task UpdateAsync(DmChannel entity, bool save = true) {
        _db.DmChannels.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<DmChannel>> FindAsync(Expression<Func<DmChannel, bool>> predicate) =>
        await _db.DmChannels.Where(predicate).ToListAsync();
}