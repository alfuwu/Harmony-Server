using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/messages")]
[Authorize]
public class MessageController(IMessageService messageService) : ControllerBase {
    private readonly IMessageService _messageService = messageService;

    [HttpGet("channel/{channelId}")]
    public async Task<IActionResult> GetMessages(long channelId) => Ok(await _messageService.GetRecentMessagesAsync(channelId));
}
