using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Server.Middleware;
public class RateLimitOptions {
    public int RequestsPerWindow { get; set; } = 30;
    public int WindowSeconds { get; set; } = 10;
}

public class RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, IOptions<RateLimitOptions> opts) {
    private readonly RequestDelegate _next = next;
    private readonly IMemoryCache _cache = cache;
    private readonly RateLimitOptions _opts = opts.Value;

    public async Task InvokeAsync(HttpContext context) {
        var key = GetClientKey(context);
        var entry = _cache.GetOrCreate(key, e => {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_opts.WindowSeconds);
            return new RequestCounter {
                Count = 0
            };
        })!;

        if (entry.Count >= _opts.RequestsPerWindow) {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests");
            return;
        }

        entry.Count++;
        _cache.Set(key, entry, TimeSpan.FromSeconds(_opts.WindowSeconds));

        await _next(context);
    }

    private static string GetClientKey(HttpContext ctx) {
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"rl_{ip}";
    }

    private class RequestCounter {
        public int Count { get; set; }
    }
}