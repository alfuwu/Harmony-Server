using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.DTOs.Output;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api")]
[Authorize]
public class MessageController(IMessageService messageService, IUserService userService) : ControllerBase {
    private readonly IMessageService _messageService = messageService;
    private readonly IUserService _userService = userService;

    [HttpGet("servers/{serverId}/channels/{channelId}/messages")]
    public async Task<IActionResult> GetMessages([FromRoute] long channelId, [FromQuery] long before = 0, [FromQuery] byte limit = 50) {
        try {
            return Ok((await _messageService.GetRecentMessagesAsync(channelId, before, limit)).Select(m => new MessageDto(m)));
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("servers/{serverId}/channels/{channelId}/messages")]
    public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto dto, [FromRoute] long channelId) {
        try {
            return Ok(new MessageDto(await _messageService.SendMessageAsync(dto, channelId, await JwtTokenHelper.GetId(_userService, User))));
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet("dms/{channelId}/messages")]
    public async Task<IActionResult> GetDMMessages([FromRoute] long channelId, [FromQuery] long before = 0, [FromQuery] byte limit = 50) {
        try {
            return Ok((await _messageService.GetRecentMessagesAsync(channelId, before, limit)).Select(m => new MessageDto(m)));
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("dms/{channelId}/messages")]
    public async Task<IActionResult> SendDMMessage([FromBody] MessageCreateDto dto, [FromRoute] long channelId) {
        try {
            return Ok(new MessageDto(await _messageService.SendMessageAsync(dto, channelId, await JwtTokenHelper.GetId(_userService, User))));
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}
