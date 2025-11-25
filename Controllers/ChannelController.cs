using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/server/{serverId}/channels")]
[Authorize]
public class ChannelController(IChannelService channelService, IUserService userService) : ControllerBase {
    private readonly IChannelService _channelService = channelService;
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetChannels([FromRoute] long serverId) => Ok(await _channelService.GetAllChannelsAsync(serverId));

    [HttpPost]
    public async Task<IActionResult> CreateChannel([FromRoute] long serverId, [FromBody] ChannelCreateDto dto) {
        try {
            return Ok(await _channelService.CreateChannelAsync(dto, await JwtTokenHelper.GetId(_userService, User), serverId));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteServer([FromBody] IdDto dto) {
        try {
            await _channelService.DeleteChannelAsync(dto, await JwtTokenHelper.GetId(_userService, User));
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}