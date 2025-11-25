using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/servers")]
[Authorize]
public class ServerController(IServerService serverService, IUserService userService) : ControllerBase {
    private readonly IServerService _serverService = serverService;
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetServers() {
        try {
            return Ok(await _serverService.GetServersAsync(await JwtTokenHelper.GetId(_userService, User)));
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateServer([FromBody] ServerCreateDto dto) {
        try {
            return Ok(await _serverService.CreateServerAsync(dto, await JwtTokenHelper.GetId(_userService, User)));
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteServer([FromBody] IdDto dto) {
        try {
            await _serverService.DeleteServerAsync(dto, await JwtTokenHelper.GetId(_userService, User));
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinServer([FromBody] IdDto dto) {
        try {
            await _serverService.JoinServerAsync(dto, await JwtTokenHelper.GetId(_userService, User));
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (InvalidOperationException e) {
            return Conflict(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}