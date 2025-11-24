using Server.Data;

namespace Server.Services.Background;
public class PresenceCleanupService(IServiceScopeFactory scopeFactory) : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            try {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // stuff idk
            } catch { }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}