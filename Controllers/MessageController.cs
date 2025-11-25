using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/")]
[Authorize]
public class MessageController(IMessageService messageService) : ControllerBase {
    private readonly IMessageService _messageService = messageService;

    [HttpGet("channel/{channelId}/messages")]
    public async Task<IActionResult> GetMessages([FromRoute] long channelId, [FromQuery] long offset = 0, [FromQuery] byte limit = 50) =>
        Ok(await _messageService.GetRecentMessagesAsync(channelId, offset, limit));
}
