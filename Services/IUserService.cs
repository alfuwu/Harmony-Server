using Server.DTOs;
using Server.Models;

namespace Server.Services;
public interface IUserService {
    Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<User?> GetByIdAsync(long id);
}