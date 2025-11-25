using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;
[ApiController]
[Route("api/invite/{inviteUrl}")]
public class InviteController(IServerService serverService) : ControllerBase {
    private readonly IServerService _serverService = serverService;

    [HttpGet]
    public async Task<IActionResult> GetInviteInfo([FromRoute] string inviteUrl) {
        try {
            GuildServer? server = await _serverService.GetServerFromInviteUrlAsync(inviteUrl.ToLower());
            return server == null ?
                NotFound(new { error = "No server found with invite URL of \"" + inviteUrl.ToLower() + "\"" }) : // this never trips bc the sql db throws an error
                Ok(server);
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }
    }
}
