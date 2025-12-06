using Server.DTOs.Input;
using Server.Models;

namespace Server.Services;
public interface IUserService {
    Task<bool> IsUsernameAvailable(string username, long? ignore = null);
    Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(RegisterDto dto);
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByIdWithSettingsAsync(long id);
    Task UpdateAsync(User user);
    Task<string?> UpdateAvatarAsync(long id, string? newAvatarHash);
    Task<string?> UpdateBannerAsync(long id, string? newBannerHash);
    Task<string?> UpdateFontAsync(long id, string? newFontHash);
    Task UpdateSettingsAsync(UserSettings settings);
}