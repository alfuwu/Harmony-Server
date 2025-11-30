using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Input;
using Server.DTOs.Output;
using Server.Helpers;
using Server.Helpers.Extensions;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/servers/{serverId}/channels")]
[Authorize]
public class ChannelController(IChannelService channelService, IUserService userService) : ControllerBase {
    private readonly IChannelService _channelService = channelService;
    private readonly IUserService _userService = userService;

    // get channels
    [HttpGet]
    public async Task<IActionResult> GetChannels([FromRoute] long serverId) {
        try {
            return Ok((await _channelService.GetAllChannelsAsync(serverId)).Select(c => new ChannelDto(c)));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }

    // create
    [HttpPost]
    public async Task<IActionResult> CreateChannel([FromBody] ChannelCreateDto dto, [FromRoute] long serverId) {
        try {
            return Ok(new ChannelDto(await _channelService.CreateChannelAsync(dto, serverId, await JwtTokenHelper.GetId(_userService, User))));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }

    // bulk delete
    [HttpDelete]
    public async Task<IActionResult> DeleteChannel([FromBody] IdDto[] dtos) {
        try {
            long userId = await JwtTokenHelper.GetId(_userService, User);
            foreach (var dto in dtos)
                await _channelService.DeleteChannelAsync(dto.Id, userId);
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }

    // delete single
    [HttpDelete("{channelId}")]
    public async Task<IActionResult> DeleteChannel([FromRoute] long channelId) {
        try {
            await _channelService.DeleteChannelAsync(channelId, await JwtTokenHelper.GetId(_userService, User));
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }

    // update
    [HttpPatch]
    public async Task<IActionResult> UpdateChannel([FromBody] ChannelCreateDto dto) {
        try {
            throw new NotImplementedException("Not implemented");
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }
}