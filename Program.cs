using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Hubs;
using Server.Middleware;
using Server.Models;
using Server.Repositories;
using Server.Services;
using Server.Services.Background;

var builder = WebApplication.CreateBuilder(args);

// config
var configuration = builder.Configuration;

// add services
builder.Services.AddControllers();
// see more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ef core - sql server (switch to InMemory for quick testing)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source=./HarmonicDb.sqlite"));
//    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// signalr
builder.Services.AddSignalR();

// repositories & services
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Channel>, ChannelRepository>();
builder.Services.AddScoped<IRepository<Message>, MessageRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// background services
builder.Services.AddHostedService<PresenceCleanupService>();

// simple in-memory rate limiter config
builder.Services.Configure<RateLimitOptions>(configuration.GetSection("RateLimiting"));

// jwt auth
var jwtSection = configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options => {
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        ValidateLifetime = true
    };

    // enable signalr to read token from querystring as "access_token"
    options.Events = new JwtBearerEvents {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"].FirstOrDefault();
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat")) {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// add MemoryCache for rate limiter/state
builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// rate limiting middleware
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();