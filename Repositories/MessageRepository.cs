using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories;
public class MessageRepository(AppDbContext db) : IRepository<Message> {
    private readonly AppDbContext _db = db;

    public async Task<Message> AddAsync(Message entity, bool save = true) {
        _db.Messages.Add(entity);
        var c = await _db.AbstractChannels.FindAsync(entity.ChannelId);
        c!.LastMessage = entity.Id;
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(Message entity, bool save = true) {
        _db.Messages.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<Message?> GetAsync(long id) =>
        await _db.Messages.Include(m => m.AuthorId).Include(m => m.ChannelId).FirstOrDefaultAsync(m => m.Id == id);
    public async Task<IEnumerable<Message>> GetAllAsync() =>
        await _db.Messages.Include(m => m.AuthorId).Include(m => m.ChannelId).ToListAsync();
    public async Task UpdateAsync(Message entity, bool save = true) {
        _db.Messages.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<Message>> FindAsync(Expression<Func<Message, bool>> predicate) =>
        await _db.Messages.Where(predicate).ToListAsync();
}