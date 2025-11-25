using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/server/{serverId}/channels")]
[Authorize]
public class ChannelController(IChannelService channelService) : ControllerBase {
    private readonly IChannelService _channelService = channelService;

    [HttpGet]
    public async Task<IActionResult> GetChannels([FromRoute] long serverId) => Ok(await _channelService.GetAllChannelsAsync(serverId));

    [HttpPost]
    public async Task<IActionResult> CreateChannel([FromBody] ChannelCreateDto dto) => Ok(await _channelService.CreateChannelAsync(dto));
}