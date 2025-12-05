using System.Net.Mail;
using Humanizer;
using Server.Data;
using Server.DTOs.Input;
using Server.Helpers;
using Server.Models;
using Server.Repositories;

using static BCrypt.Net.BCrypt;

namespace Server.Services;
public class UserService(IRepository<User> users, IConfiguration configuration, AppDbContext db) : IUserService {
    private readonly IRepository<User> _users = users;
    private readonly IConfiguration _configuration = configuration;
    private readonly AppDbContext _db = db;

    public async Task<bool> IsUsernameAvailable(string username, long? ignore = null) => !(await _users.FindAsync(u => u.Id != ignore && u.Username == username)).Any();

    private async Task ValidateData(User user, long? ignore = null) {
        if (user.IsDeleted)
            throw new ArgumentException("User does not exist");

        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentException("Username must be provided");
        if (user.Username.Length is < 2 or > 32)
            throw new ArgumentException("Username must be between 2 and 32 characters long");
        if (user.Username.Contains(' '))
            throw new ArgumentException("Username contains invalid characters");

        var exists = !await IsUsernameAvailable(user.Username, ignore);
        if (exists)
            throw new ArgumentException("Username already exists");

        if (string.IsNullOrWhiteSpace(user.Email) || !IsValidEmail(user.Email))
            throw new ArgumentException("Invalid email");

        var emailExists = (await _users.FindAsync(u => u.Id != ignore && u.Email == user.Email)).Any();
        if (emailExists)
            throw new ArgumentException("An account already exists with that email address");

        if (user.DisplayName != null && user.DisplayName.Length is < 2 or > 32)
            throw new ArgumentException("Display name must be between 2 and 32 characters long");

        if (user.DmColors != null && user.DmColors.Count > 6)
            throw new ArgumentException("Name color gradient can only have at most 6 different colors");

        if (user.Bio != null && user.Bio.Length > 4096)
            throw new ArgumentException("Bio cannot exceed 4096 characters");
    }

    public async Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto) {
        dto.Username = dto.Username.Trim();//.ToLowerInvariant();
        dto.Email = dto.Email.ToLowerInvariant();

        if (dto.Password.Length < 5)
            throw new ArgumentException("Password must be at least 5 characters long");

        var user = new User {
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            JoinedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        await ValidateData(user);

        await _users.AddAsync(user);

        user.Settings = new UserSettings {
            UserId = user.Id
        };
        await _db.UserSettings.AddAsync(user.Settings);
        await _db.SaveChangesAsync();

        return new RegistrationCompleteDto {
            Id = user.Id,
            Username = user.Username,
            Token = JwtTokenHelper.GenerateToken(user, _configuration)
        };
    }

    private static bool IsValidEmail(string email) {
        try {
            var addr = new MailAddress(email);
            return addr.Address == email;
        } catch {
            return false;
        }
    }

    public async Task<string> LoginAsync(RegisterDto dto) {
        var found = (await _users.FindAsync(u => u.Username == dto.Username ||
            u.Email == dto.Username)).FirstOrDefault();
        if (found == null || !Verify(dto.Password, found.PasswordHash))
            throw new InvalidDataException("Invalid credentials");

        found.LastSeen = DateTime.UtcNow;
        await _users.UpdateAsync(found);
        return JwtTokenHelper.GenerateToken(found, _configuration);
    }

    public async Task<User?> GetByIdAsync(long id) => await _users.GetAsync(id);

    public async Task<User?> GetByIdWithSettingsAsync(long id) => await _users.GetExhaustiveAsync(id);

    public async Task UpdateAsync(User user) {
        await ValidateData(user, user.Id);
        await _users.UpdateAsync(user);
    }

    public async Task<string?> UpdateAvatarAsync(long userId, string? newAvatarHash) {
        var user = await GetByIdAsync(userId) ?? throw new KeyNotFoundException("User does not exist");
        var oldAvatar = user.Avatar;

        user.Avatar = newAvatarHash;
        await _users.UpdateAsync(user);

        return oldAvatar;
    }

    public async Task<string?> UpdateBannerAsync(long userId, string? newBannerHash) {
        var user = await GetByIdAsync(userId) ?? throw new KeyNotFoundException("User does not exist");
        var oldAvatar = user.Banner;

        user.Banner = newBannerHash;
        await _users.UpdateAsync(user);

        return oldAvatar;
    }

    public async Task UpdateSettingsAsync(UserSettings newSettings) {
        _db.UserSettings.Update(newSettings);
        await _db.SaveChangesAsync();
    }
}