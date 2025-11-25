using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories.PrivateChannels;
public class GroupDMChannelRepository(AppDbContext db) : IRepository<GroupDMChannel> {
    private readonly AppDbContext _db = db;

    public async Task<GroupDMChannel> AddAsync(GroupDMChannel entity, bool save = true) {
        _db.GroupDMChannels.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(GroupDMChannel entity, bool save = true) {
        _db.GroupDMChannels.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<GroupDMChannel?> GetAsync(long id) => await _db.GroupDMChannels.FindAsync(id);
    public async Task<IEnumerable<GroupDMChannel>> GetAllAsync() => await _db.GroupDMChannels.ToListAsync();
    public async Task<IEnumerable<GroupDMChannel>> GetForIdAsync(long userId) =>
        await _db.GroupDMChannels.Where(c => c.Members.Contains(userId)).ToListAsync();
    public async Task UpdateAsync(GroupDMChannel entity, bool save = true) {
        _db.GroupDMChannels.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<GroupDMChannel>> FindAsync(Expression<Func<GroupDMChannel, bool>> predicate) =>
        await _db.GroupDMChannels.Where(predicate).ToListAsync();
}