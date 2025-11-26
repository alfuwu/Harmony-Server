using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase {
    private readonly IUserService _userService = userService;

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
            return Ok(await _userService.GetByIdAsync(userId));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}