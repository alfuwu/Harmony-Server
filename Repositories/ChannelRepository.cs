using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories;
public class ChannelRepository(AppDbContext db) : IRepository<Channel> {
    private readonly AppDbContext _db = db;

    public async Task<Channel> AddAsync(Channel entity) {
        _db.Channels.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(Channel entity) {
        _db.Channels.Remove(entity);
        await _db.SaveChangesAsync();
    }
    public async Task<Channel?> GetAsync(long id) => await _db.Channels.FindAsync(id);
    public async Task<IEnumerable<Channel>> GetAllAsync() => await _db.Channels.ToListAsync();
    public async Task UpdateAsync(Channel entity) {
        _db.Channels.Update(entity);
        await _db.SaveChangesAsync();
    }
    public async Task<IEnumerable<Channel>> FindAsync(Expression<Func<Channel, bool>> predicate) =>
        await _db.Channels.Where(predicate).ToListAsync();
}