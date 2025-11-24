using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : ControllerBase {
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto) {
        try {
            var user = await _userService.RegisterAsync(dto);
            return Ok(user);
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) {
        try {
            var token = await _userService.LoginAsync(dto);
            return Ok(new { token });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}
