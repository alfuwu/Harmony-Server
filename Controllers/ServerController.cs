using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Input;
using Server.DTOs.Output;
using Server.Helpers;
using Server.Models.Enums;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/servers")]
[Authorize]
public class ServerController(IServerService serverService, IChannelService channelService, IUserService userService, IRelationshipService relationshipService) : ControllerBase {
    private readonly IServerService _serverService = serverService;
    private readonly IChannelService _channelService = channelService;
    private readonly IUserService _userService = userService;
    private readonly IRelationshipService _relationshipService = relationshipService;

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
            var s = new ServerDto(await _serverService.CreateServerAsync(dto, await JwtTokenHelper.GetId(_userService, User)));
            await _channelService.CreateChannelAsync(new ChannelCreateDto {
                Name = "general",
                Description = "Automatically generated default channel",
                Type = ChannelType.Text
            }, s.Id, s.OwnerId);
            return Ok(s);
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
            long requestorId = await JwtTokenHelper.GetId(_userService, User);
            return Ok((await _serverService.GetMembersAsync(serverId, requestorId, page, pageSize)).Select(async m => {
                var dto = new MemberDto(m);
                await dto.Redact(_relationshipService, m.User, requestorId);
                return dto;
            }));
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