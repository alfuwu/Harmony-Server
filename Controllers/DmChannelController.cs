using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Input;
using Server.Helpers;
using Server.Helpers.Extensions;
using Server.Models;
using Server.Models.Enums;
using Server.Services;
using Server.Services.PrivateChannels;

namespace Server.Controllers;
[ApiController]
[Route("api/dms")]
[Authorize]
public class DmChannelController(IDmChannelService dmChannelService, IGroupDmChannelService groupDmChannelService, IUserService userService) : ControllerBase {
    private readonly IDmChannelService _dmChannelService = dmChannelService;
    private readonly IGroupDmChannelService _groupDmChannelService = groupDmChannelService;
    private readonly IUserService _userService = userService;

    // get dms
    [HttpGet]
    public async Task<IActionResult> GetChannels() {
        try {
            long userId = await JwtTokenHelper.GetId(_userService, User);
            return Ok(new List<object>(await _dmChannelService.GetAllChannelsAsync(userId))
                .Concat(await _groupDmChannelService.GetAllChannelsAsync(userId)));
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
    public async Task<IActionResult> CreateChannel([FromBody] PrivateChannelCreateDto dto) {
        try {
            long userId = await JwtTokenHelper.GetId(_userService, User);
            if (dto.Type == ChannelType.DM)
                return Ok(await _dmChannelService.CreateChannelAsync(dto, userId));
            else
                return Ok(await _groupDmChannelService.CreateChannelAsync(dto, userId));
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }

    // delete dm
    [HttpDelete("{channelId}")]
    public async Task<IActionResult> DeleteChannel([FromRoute] long channelId, [FromQuery] bool isGdm) {
        try {
            long userId = await JwtTokenHelper.GetId(_userService, User);
            if (isGdm)
                await _groupDmChannelService.DeleteChannelAsync(channelId, userId);
            else
                await _dmChannelService.DeleteChannelAsync(channelId, userId);
            return Ok();
        } catch (KeyNotFoundException e) {
            return NotFound(new { error = e.Message });
        } catch (UnauthorizedAccessException e) {
            return Unauthorized(new { error = e.Message });
        } catch (Exception e) {
            return this.InternalServerError(new { error = e.Message });
        }
    }
}