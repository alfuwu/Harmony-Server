using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/channel")]
[Authorize]
public class ChannelController(IChannelService channelService) : ControllerBase {
    private readonly IChannelService _channelService = channelService;

    [HttpGet]
    public async Task<IActionResult> GetChannels() => Ok(await _channelService.GetAllChannelsAsync());

    [HttpPost]
    public async Task<IActionResult> CreateChannel([FromBody] ChannelDto dto) => Ok(await _channelService.CreateChannelAsync(dto));
}