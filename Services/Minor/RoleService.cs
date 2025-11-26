using Server.DTOs.Input;
using Server.Models;
using Server.Repositories;

namespace Server.Services.Minor;
public class RoleService(IRepository<Role> roles, IRepository<GuildServer> servers) : IRoleService {
    private readonly IRepository<Role> _roles = roles;
    private readonly IRepository<GuildServer> _servers = servers;

    public async Task<Role> CreateRoleAsync(RoleCreateDto dto, long userId) {
        var server = await _servers.GetAsync(dto.ServerId) ?? throw new KeyNotFoundException("Server not found");
        if (server.OwnerId != userId && userId > 0) // check for role management perms here
            throw new UnauthorizedAccessException("You cannot create a role in this server");

        var r = new Role {
            ServerId = dto.ServerId,
            Name = dto.Name,
            Description = dto.Description
        };
        return await _roles.AddAsync(r);
    }

    public async Task DeleteRoleAsync(IdDto dto, long userId) {
        var r = await _roles.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Role not found");
        var server = await _servers.GetAsync(r.ServerId) ?? throw new KeyNotFoundException("Server not found (???)");
        if (server.OwnerId != userId && userId > 0) // check for channel management perms here
            throw new UnauthorizedAccessException("You cannot delete this channel");

        await _roles.DeleteAsync(r);
    }

    public async Task<Role?> GetByIdAsync(long id) => await _roles.GetAsync(id);
}