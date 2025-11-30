using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.DTOs.Output;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/channels/{channelId}/messages")]
[Authorize]
public class MessageController(IMessageService messageService, IUserService userService) : ControllerBase {
    private readonly IMessageService _messageService = messageService;
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetMessages([FromRoute] long channelId, [FromQuery] long before = 0, [FromQuery] byte limit = 50) {
        try {
            return Ok((await _messageService.GetRecentMessagesAsync(channelId, before, limit)).Select(m => new MessageDto(m)));
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto dto, [FromRoute] long channelId) {
        try {
            var m = await _messageService.SendMessageAsync(dto, channelId, await JwtTokenHelper.GetId(_userService, User));
            return Ok(new {
                m.Id,
                m.ChannelId,
                m.AuthorId,
                m.Mentions,
                m.Reactions,
                m.Content,
                m.PreviousContent,
                m.References,
                m.Timestamp,
                m.EditedTimestamp,
                m.IsDeleted,
                m.IsPinned,
                dto.Nonce // we need to send the nonce back so that the client knows what message to replace
            });
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}
