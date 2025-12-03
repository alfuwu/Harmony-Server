using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Server.Data;
using Server.DTOs.Input;
using Server.Models;
using static System.IO.File;

namespace Server.Controllers;
[Authorize]
[ApiController]
[Route("/api/attachments")]
public class AttachmentsController : ControllerBase {
    private readonly string storagePath;
    private readonly AppDbContext _db;
    private static readonly FileExtensionContentTypeProvider provider = new();

    public AttachmentsController(IWebHostEnvironment env, AppDbContext db) {
        storagePath = Path.Combine(env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(storagePath);
        _db = db;
    }

    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50mb limit
    public async Task<IActionResult> Upload([FromForm] FileDto dto) {
        var file = dto.File;
        var uploadsDir = Path.Combine("Uploads");
        Directory.CreateDirectory(uploadsDir);

        string name = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsDir, name);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var attachment = new Attachment {
            FileName = file.FileName,
            FilePath = filePath,
            MessageId = null,
            UploadedAt = DateTime.UtcNow
        };

        _db.Attachments.Add(attachment);
        await _db.SaveChangesAsync();

        return Ok(new { attachment.Id, name });
    }

    [AllowAnonymous]
    [HttpGet("{fileName}")]
    public async Task<IActionResult> Download(string fileName) {
        var filePath = Path.Combine(storagePath, fileName);

        if (!Exists(filePath))
            return NotFound();

        if (!provider.TryGetContentType(fileName, out var contentType))
            contentType = "application/octet-stream";

        var content = await ReadAllBytesAsync(filePath);
        return File(content, contentType, fileName);
    }
}
