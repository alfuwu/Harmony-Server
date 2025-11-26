using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories.PrivateChannels;
public class GroupDmChannelRepository(AppDbContext db) : IRepository<GroupDmChannel> {
    private readonly AppDbContext _db = db;

    public async Task<GroupDmChannel> AddAsync(GroupDmChannel entity, bool save = true) {
        _db.GroupDmChannels.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(GroupDmChannel entity, bool save = true) {
        _db.GroupDmChannels.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<GroupDmChannel?> GetAsync(long id) => await _db.GroupDmChannels.FindAsync(id);
    public async Task<IEnumerable<GroupDmChannel>> GetAllAsync() => await _db.GroupDmChannels.ToListAsync();
    public async Task<IEnumerable<GroupDmChannel>> GetForIdAsync(long userId) =>
        await _db.GroupDmChannels.Where(c => c.Members.Contains(userId)).ToListAsync();
    public async Task UpdateAsync(GroupDmChannel entity, bool save = true) {
        _db.GroupDmChannels.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<GroupDmChannel>> FindAsync(Expression<Func<GroupDmChannel, bool>> predicate) =>
        await _db.GroupDmChannels.Where(predicate).ToListAsync();
}