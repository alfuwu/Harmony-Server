using Server.DTOs;
using Server.Helpers;
using Server.Models;
using Server.Repositories;

using static BCrypt.Net.BCrypt;

namespace Server.Services;
public class UserService(IRepository<User> users, IConfiguration configuration) : IUserService {
    private readonly IRepository<User> _users = users;
    private readonly IConfiguration _configuration = configuration;

    public async Task<UserDto> RegisterAsync(RegisterDto dto) {
        if (dto.Username?.Length < 2)
            throw new Exception("Username is too short");
        else if (dto.Username?.Length > 25)
            throw new Exception("Username is too long");

        var exists = (await _users.FindAsync(u => u.Username == dto.Username)).Any();
        if (exists)
            throw new Exception("Username already exists");

        var user = new User {
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            LastSeen = DateTime.UtcNow
        };
        await _users.AddAsync(user);
        return new UserDto {
            Id = user.Id,
            Username = user.Username
        };
    }

    public async Task<string> LoginAsync(LoginDto dto) {
        var found = (await _users.FindAsync(u => u.Username == dto.Username)).FirstOrDefault();
        if (found == null || !Verify(dto.Password, found.PasswordHash))
            throw new Exception("Invalid credentials");

        found.LastSeen = DateTime.UtcNow;
        await _users.UpdateAsync(found);
        return JwtTokenHelper.GenerateToken(found, _configuration);
    }

    public async Task<UserDto?> GetByIdAsync(long id) {
        var u = await _users.GetAsync(id);
        if (u == null)
            return null;

        return new UserDto {
            Id = u.Id,
            Username = u.Username
        };
    }
}