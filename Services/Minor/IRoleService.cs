using Server.DTOs.Input;
using Server.Models;

namespace Server.Services.Minor;
public interface IRoleService {
    Task<Role> CreateRoleAsync(RoleCreateDto dto, long userId);
    Task DeleteRoleAsync(IdDto dto, long userId);
    Task<Role?> GetByIdAsync(long id);
}