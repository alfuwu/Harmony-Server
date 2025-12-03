using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Input;
using Server.DTOs.Output;
using Server.Helpers;
using Server.Models;
using Server.Services;
using static System.IO.File;
using static BCrypt.Net.BCrypt;

namespace Server.Controllers;
[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase {
    private readonly string avatarPath;
    private readonly string bannerPath;
    private readonly IUserService _userService;
    private readonly IServerService _serverService;
    private readonly IRelationshipService _relationshipService;

    public UserController(IWebHostEnvironment env, IUserService userService, IServerService serverService, IRelationshipService relationshipService) {
        avatarPath = Path.Combine(env.ContentRootPath, "Avatars");
        bannerPath = Path.Combine(env.ContentRootPath, "Banners");
        Directory.CreateDirectory(avatarPath);
        Directory.CreateDirectory(bannerPath);
        _userService = userService;
        _serverService = serverService;
        _relationshipService = relationshipService;
    }

    [HttpGet("@me")]
    public async Task<IActionResult> GetMe() {
        try {
            return Ok(await _userService.GetByIdAsync(await JwtTokenHelper.GetId(_userService, User)));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser([FromRoute] long userId) {
        try {
            long? requestorId = null;
            try {
                requestorId = await JwtTokenHelper.GetId(_userService, User);
            } catch (UnauthorizedAccessException) { }

            var user = await _userService.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            var dto = new UserDto(user);
            await dto.Redact(_relationshipService, user, requestorId);
            return Ok(dto);
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("@me/settings")]
    public async Task<IActionResult> GetSettings() {
        try {
            var userId = await JwtTokenHelper.GetId(_userService, User);
            var user = await _userService.GetByIdWithSettingsAsync(userId);

            if (user == null)
                return NotFound(new { error = "User not found" });

            if (user.Settings == null)
                return NotFound(new { error = "Settings not found" });

            user.Settings.User = null; // prevent serializing the user (dirty hack to get out of providing a dto)

            return Ok(user.Settings);
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPatch("@me/settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UserSettings settings) {
        try {
            var userId = await JwtTokenHelper.GetId(_userService, User);
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { error = "User not found" });

            settings.UserId = userId;
            settings.User = user;

            await _userService.UpdateSettingsAsync(settings);
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPatch("@me/profile/{serverId}")]
    public async Task<IActionResult> UpdateMemberProfile([FromRoute] long serverId, [FromBody] MemberUpdateDto update) {
        try {
            var id = await JwtTokenHelper.GetId(_userService, User);

            var member = await _serverService.GetMemberAsync(serverId, id);
            if (member == null)
                return NotFound(new { error = "You are not a member of this server" });

            member.Nickname = update.Nickname ?? member.Nickname;
            member.Pronouns = update.Pronouns ?? member.Pronouns;
            member.Bio = update.Bio ?? member.Bio;
            //member.Avatar = update.Avatar ?? member.Avatar;
            //member.Banner = update.Banner ?? member.Banner;
            //member.NameFont = update.NameFont ?? member.NameFont;

            await _serverService.UpdateMemberAsync(member);
            return Ok();
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPatch("@me/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto update) {
        try {
            var id = await JwtTokenHelper.GetId(_userService, User);
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { error = "User not found" });

            user.DisplayName = update.DisplayName ?? user.DisplayName;
            user.Pronouns = update.Pronouns ?? user.Pronouns;
            user.Status = update.Status ?? user.Status;
            user.Bio = update.Bio ?? user.Bio;

            await _userService.UpdateAsync(user);
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    // TODO: make more secure???
    // TODO: heavily rate limit
    [HttpPatch("@me")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto update) {
        try {
            var id = await JwtTokenHelper.GetId(_userService, User);
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { error = "User not found" });

            if (!string.IsNullOrWhiteSpace(update.Email)) {
                user.Email = update.Email;
                user.EmailVerified = false;
            }

            if (!string.IsNullOrWhiteSpace(update.PhoneNumber)) {
                user.PhoneNumber = update.PhoneNumber;
                user.PhoneNumberVerified = false;
            }

            user.Username = update.Username ?? user.Username;

            if (!string.IsNullOrWhiteSpace(update.Password)) {
                if (update.Password.Length < 5)
                    throw new ArgumentException("Password must be at least 5 characters long");
                user.PasswordHash = HashPassword(update.Password);
            }

            await _userService.UpdateAsync(user);
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    private static string FormatImagePath(long id, string uuid) => $"{id}_{uuid}";
    private static string FormatImagePath(long id, Guid uuid) => $"{id}_{uuid:N}";

    [HttpPost("@me/avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5mb size limit
    public async Task<IActionResult> UploadAvatar([FromForm] FileDto dto) {
        try {
            var file = dto.File;
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var userId = await JwtTokenHelper.GetId(_userService, User);
            var avatarId = FormatImagePath(userId, Guid.NewGuid());
            var path = Path.Combine(avatarPath, avatarId);
            await FileHelper.UploadImage(file, path);

            var oldAvatar = await _userService.UpdateAvatarAsync(userId, avatarId);

            // delete old avatar file
            if (!string.IsNullOrWhiteSpace(oldAvatar)) {
                var oldPattern = Directory.GetFiles(avatarPath, oldAvatar + ".*");
                foreach (var oldFile in oldPattern)
                    Delete(oldFile);
            }

            return Ok(new { avatar = avatarId });
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("@me/banner")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5mb size limit
    public async Task<IActionResult> UploadBanner([FromForm] FileDto dto) {
        try {
            var file = dto.File;
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var userId = await JwtTokenHelper.GetId(_userService, User);
            var bannerId = FormatImagePath(userId, Guid.NewGuid());
            var path = Path.Combine(bannerPath, bannerId);
            await FileHelper.UploadImage(file, path);

            var oldBanner = await _userService.UpdateBannerAsync(userId, bannerId);

            if (!string.IsNullOrWhiteSpace(oldBanner)) {
                var oldPattern = Directory.GetFiles(bannerPath, oldBanner + ".*");
                foreach (var oldFile in oldPattern)
                    Delete(oldFile);
            }

            return Ok(new { banner = bannerId });
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("{userId}/avatar/{avatar}")]
    public async Task<IActionResult> GetAvatar([FromRoute] long userId, [FromRoute] string avatar) {
        var pattern = Directory.GetFiles(avatarPath, FormatImagePath(userId, avatar) + ".*");

        if (pattern.Length == 0)
            return NotFound();

        var filePath = pattern[0];
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        var contentType =
            ext == ".png" ? "image/png" :
            ext == ".jpg" || ext == ".jpeg" ? "image/jpeg" :
            ext == ".webp" ? "image/webp" :
            ext == ".gif" ? "image/gif" :
            "application/octet-stream";

        var bytes = await ReadAllBytesAsync(filePath);
        return File(bytes, contentType);
    }

    [AllowAnonymous]
    [HttpGet("{userId}/banner/{banner}")]
    public async Task<IActionResult> GetBanner([FromRoute] long userId, [FromRoute] string banner) {
        var pattern = Directory.GetFiles(bannerPath, FormatImagePath(userId, banner) + ".*");

        if (pattern.Length == 0)
            return NotFound();

        var filePath = pattern[0];
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        var contentType =
            ext == ".png" ? "image/png" :
            ext == ".jpg" || ext == ".jpeg" ? "image/jpeg" :
            ext == ".webp" ? "image/webp" :
            ext == ".gif" ? "image/gif" :
            "application/octet-stream";

        var bytes = await ReadAllBytesAsync(filePath);
        return File(bytes, contentType);
    }

    [HttpPost("add-friend")]
    public async Task<IActionResult> AddFriend([FromBody] long targetId) {
        try {
            var id = await JwtTokenHelper.GetId(_userService, User);

            var relationshipType = await _relationshipService.GetRelationship(id, targetId);
            var target = await _userService.GetByIdAsync(targetId);
            if (target == null)
                return NotFound(new { error = "Target not found" });

            await _relationshipService.SendFriendRequest(id, targetId, target.WhoCanSendFriendRequests);
            return Ok();
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("accept-friend")]
    public async Task<IActionResult> AcceptFriend([FromBody] long sourceId) {
        try {
            var id = await JwtTokenHelper.GetId(_userService, User);

            await _relationshipService.AcceptFriendRequest(sourceId, id);
            return Ok();
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends() {
        try {
            var id = await JwtTokenHelper.GetId(_userService, User);
            return Ok(await _relationshipService.GetRelationships(id));
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}