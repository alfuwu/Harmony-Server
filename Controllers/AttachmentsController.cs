using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;
using static System.IO.File;

namespace Server.Controllers;
[ApiController]
[Route("/api/attachments")]
public class AttachmentsController : ControllerBase {
    private readonly string storagePath;
    private readonly AppDbContext _db;

    public AttachmentsController(IWebHostEnvironment env, AppDbContext db) {
        storagePath = Path.Combine(env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(storagePath);
        _db = db;
    }

    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50mb limit
    public async Task<IActionResult> Upload([FromForm] IFormFile file) {
        var uploadsDir = Path.Combine("Uploads");
        Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, Guid.NewGuid().ToString() + "_" + file.FileName);

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

        return Ok(new { attachment.Id });
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> Download(string fileName) {
        var filePath = Path.Combine(storagePath, fileName);

        if (!Exists(filePath))
            return NotFound();

        var content = await ReadAllBytesAsync(filePath);
        return File(content, "application/octet-stream", fileName);
    }
}
