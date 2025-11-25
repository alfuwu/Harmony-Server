using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories;
public class ThreadRepository(AppDbContext db) : IRepository<ThreadChannel> {
    private readonly AppDbContext _db = db;

    public async Task<ThreadChannel> AddAsync(ThreadChannel entity, bool save = true) {
        _db.Threads.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(ThreadChannel entity, bool save = true) {
        _db.Threads.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<ThreadChannel?> GetAsync(long id) => await _db.Threads.FindAsync(id);
    public async Task<IEnumerable<ThreadChannel>> GetAllAsync() => await _db.Threads.ToListAsync();
    public async Task<IEnumerable<ThreadChannel>> GetForIdAsync(long channelId) =>
        await _db.Threads.Where(c => c.ParentId == channelId).ToListAsync();
    public async Task UpdateAsync(ThreadChannel entity, bool save = true) {
        _db.Threads.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<ThreadChannel>> FindAsync(Expression<Func<ThreadChannel, bool>> predicate) =>
        await _db.Threads.Where(predicate).ToListAsync();
}