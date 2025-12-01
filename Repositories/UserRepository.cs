using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories;
public class UserRepository(AppDbContext db) : IRepository<User> {
    private readonly AppDbContext _db = db;

    public async Task<User> AddAsync(User entity, bool save = true) {
        _db.Users.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(User entity, bool save = true) {
        _db.Users.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<User?> GetAsync(long id) => await _db.Users.FindAsync(id);
    public async Task<User?> GetExhaustiveAsync(long id) => await _db.Users.Include(u => u.Settings).FirstOrDefaultAsync(u => u.Id == id);
    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _db.Users.ToListAsync();
    public async Task UpdateAsync(User entity, bool save = true) {
        _db.Users.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate) =>
        await _db.Users.Where(predicate).ToListAsync();
}