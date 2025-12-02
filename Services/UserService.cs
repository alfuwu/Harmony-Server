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

    public async Task<bool> IsUsernameAvailable(string username) => !(await _users.FindAsync(u => u.Username == username)).Any();

    public async Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto) {
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("Username must be provided");
        if (dto.Username.Length is < 2 or > 32)
            throw new ArgumentException("Username must be between 2 and 32 characters long");
        if (dto.Username.Contains(' '))
            throw new ArgumentException("Username contains invalid characters");
        dto.Username = dto.Username.Trim();//.ToLowerInvariant();

        var exists = !await IsUsernameAvailable(dto.Username);
        if (exists)
            throw new ArgumentException("Username already exists");

        if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
            throw new ArgumentException("Invalid email");
        dto.Email = dto.Email.ToLowerInvariant();

        var emailExists = (await _users.FindAsync(u => u.Email == dto.Email)).Any();
        if (emailExists)
            throw new ArgumentException("An account already exists with that email address");

        var user = new User {
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            JoinedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

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

    public async Task<string?> UpdateAvatarAsync(long userId, string newAvatarHash) {
        var user = await GetByIdAsync(userId) ?? throw new KeyNotFoundException("User does not exist");
        var oldAvatar = user.Avatar;

        user.Avatar = newAvatarHash;
        await _users.UpdateAsync(user);

        return oldAvatar;
    }

    public async Task<string?> UpdateBannerAsync(long userId, string newBannerHash) {
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