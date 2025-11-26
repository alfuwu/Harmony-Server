using Server.DTOs.Input;
using Server.Models;

namespace Server.Services;
public interface IUserService {
    Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(RegisterDto dto);
    Task<User?> GetByIdAsync(long id);
}