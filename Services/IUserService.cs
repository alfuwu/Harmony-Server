using Server.DTOs;

namespace Server.Services;
public interface IUserService {
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<UserDto?> GetByIdAsync(long id);
}