using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Services.Background;
public class AttachmentCleanupService(IServiceScopeFactory scopeFactory) : BackgroundService {
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;
    private readonly TimeSpan interval = TimeSpan.FromMinutes(15); // run periodically

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoff = DateTime.UtcNow.AddHours(-2);

            var staleAttachments = await db.Attachments
                .Where(a => a.MessageId == null && a.UploadedAt < cutoff)
                .ToListAsync(stoppingToken);

            foreach (var attachment in staleAttachments) {
                if (File.Exists(attachment.FilePath))
                    File.Delete(attachment.FilePath);

                db.Attachments.Remove(attachment);
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(interval, stoppingToken);
        }
    }
}
