using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.DTOs.Output;
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
            return Ok((await _serverService.GetServersAsync(await JwtTokenHelper.GetId(_userService, User))).Select(s => new ServerDto(s)));
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateServer([FromBody] ServerCreateDto dto) {
        try {
            return Ok(new ServerDto(await _serverService.CreateServerAsync(dto, await JwtTokenHelper.GetId(_userService, User))));
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpDelete("{serverId}")]
    public async Task<IActionResult> DeleteServer([FromRoute] long serverId) {
        try {
            await _serverService.DeleteServerAsync(serverId, await JwtTokenHelper.GetId(_userService, User));
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("{serverId}/members")]
    public async Task<IActionResult> GetMembers([FromRoute] long serverId, [FromQuery] int page = 0, [FromQuery] int pageSize = 50) {
        try {
            return Ok((await _serverService.GetMembersAsync(serverId, await JwtTokenHelper.GetId(_userService, User), page, pageSize)).Select(m => new MemberDto(m)));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("{serverId}/join")]
    public async Task<IActionResult> JoinServer([FromRoute] long serverId) {
        try {
            await _serverService.JoinServerAsync(serverId, await JwtTokenHelper.GetId(_userService, User));
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