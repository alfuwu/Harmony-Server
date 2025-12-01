using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Output;
using Server.Helpers;
using Server.Services;
using static System.IO.File;

namespace Server.Controllers;
[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase {
    private static readonly string[] ALLOWED_IMAGE_TYPES = ["image/png", "image/jpeg", "image/webp", "image/gif"];

    private readonly string avatarPath;
    private readonly string bannerPath;
    private readonly IUserService _userService;
    private readonly IRelationshipService _relationshipService;

    public UserController(IWebHostEnvironment env, IUserService userService, IRelationshipService relationshipService) {
        avatarPath = Path.Combine(env.ContentRootPath, "Avatars");
        bannerPath = Path.Combine(env.ContentRootPath, "Banners");
        Directory.CreateDirectory(avatarPath);
        Directory.CreateDirectory(bannerPath);
        _userService = userService;
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
    public async Task<IActionResult> CreateChannel([FromRoute] long userId) {
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

    [HttpPost("@me/avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5mb size limit
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file) {
        try {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var userId = await JwtTokenHelper.GetId(_userService, User);

            if (!ALLOWED_IMAGE_TYPES.Contains(file.ContentType))
                return BadRequest(new { error = "Unsupported file type" });

            var avatarId = Guid.NewGuid().ToString("N");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".png"; // default fallback

            var filePath = Path.Combine(avatarPath, avatarId + ext);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

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
    public async Task<IActionResult> UploadBanner([FromForm] IFormFile file) {
        try {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var userId = await JwtTokenHelper.GetId(_userService, User);

            if (!ALLOWED_IMAGE_TYPES.Contains(file.ContentType))
                return BadRequest(new { error = "Unsupported file type" });

            var bannerId = Guid.NewGuid().ToString("N");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".png"; // default fallback

            var filePath = Path.Combine(bannerPath, bannerId + ext);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

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

    // TODO: add some verification maybe
    [AllowAnonymous]
    [HttpGet("{userId}/avatar")]
    public async Task<IActionResult> GetAvatar(long userId) {
        var user = await _userService.GetByIdAsync(userId);

        if (user == null || string.IsNullOrWhiteSpace(user.Avatar))
            return NotFound();

        var pattern = Directory.GetFiles(avatarPath, user.Avatar + ".*");

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
    [HttpGet("{userId}/banner")]
    public async Task<IActionResult> GetBanner(long userId) {
        var user = await _userService.GetByIdAsync(userId);

        if (user == null || string.IsNullOrWhiteSpace(user.Banner))
            return NotFound();

        var pattern = Directory.GetFiles(bannerPath, user.Banner + ".*");

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
}