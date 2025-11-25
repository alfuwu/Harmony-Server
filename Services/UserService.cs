using Server.DTOs;
using Server.Helpers;
using Server.Models;
using Server.Repositories;

using static BCrypt.Net.BCrypt;

namespace Server.Services;
public class UserService(IRepository<User> users, IConfiguration configuration) : IUserService {
    private readonly IRepository<User> _users = users;
    private readonly IConfiguration _configuration = configuration;

    public async Task<RegistrationCompleteDto> RegisterAsync(RegisterDto dto) {
        if (dto.Username == null)
            throw new InvalidDataException("Username must be provided");
        if (dto.Username.Length < 2)
            throw new InvalidDataException("Username is too short");
        else if (dto.Username.Length > 32)
            throw new InvalidDataException("Username is too long");
        //dto.Username = dto.Username.ToLower();

        var exists = (await _users.FindAsync(u => u.Username == dto.Username)).Any();
        if (exists)
            throw new Exception("Username already exists");

        var user = new User {
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            LastSeen = DateTime.UtcNow
        };
        await _users.AddAsync(user);
        return new RegistrationCompleteDto {
            Id = user.Id,
            Username = user.Username,
            Token = JwtTokenHelper.GenerateToken(user, _configuration)
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

    public async Task<User?> GetByIdAsync(long id) => await _users.GetAsync(id);
}