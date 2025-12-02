using Server.DTOs.Input;
using Server.Models;

namespace Server.Services;
public interface IUserService {
    Task<bool> IsUsernameAvailable(string username);
    Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(RegisterDto dto);
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByIdWithSettingsAsync(long id);
    Task<string?> UpdateAvatarAsync(long id, string newAvatarHash);
    Task<string?> UpdateBannerAsync(long id, string newBannerHash);
    Task UpdateSettingsAsync(UserSettings settings);
}