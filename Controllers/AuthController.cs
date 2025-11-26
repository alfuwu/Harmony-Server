using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Input;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : ControllerBase {
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto) {
        try {
            return Ok(await _userService.RegisterAsync(dto));
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegisterDto dto) {
        try {
            return Ok(new { token = await _userService.LoginAsync(dto) });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}
