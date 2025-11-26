using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repositories.Minor;
public class RoleRepository(AppDbContext db) : IRepository<Role> {
    private readonly AppDbContext _db = db;

    public async Task<Role> AddAsync(Role entity, bool save = true) {
        _db.Roles.Add(entity);
        if (save)
            await _db.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(Role entity, bool save = true) {
        _db.Roles.Remove(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task<Role?> GetAsync(long id) => await _db.Roles.FindAsync(id);
    public async Task<IEnumerable<Role>> GetAllAsync() =>
        await _db.Roles.ToListAsync();
    public async Task UpdateAsync(Role entity, bool save = true) {
        _db.Roles.Update(entity);
        if (save)
            await _db.SaveChangesAsync();
    }
    public async Task SaveAsync() => await _db.SaveChangesAsync();
    public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate) =>
        await _db.Roles.Where(predicate).ToListAsync();
}