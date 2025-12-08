using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Server.Data;
using Server.Hubs;
using Server.Middleware;
using Server.Models;
using Server.Repositories;
using Server.Repositories.PrivateChannels;
using Server.Services;
using Server.Services.Background;
using Server.Services.PrivateChannels;

var builder = WebApplication.CreateBuilder(args);

// config
var configuration = builder.Configuration;

// add services
builder.Services.AddControllers();
// see more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "Server API", Version = "v1" });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token like this: Bearer {token}"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement {
        {
            new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, doc),
            new List<string>()
        }
    });
});

// ef core - sql server (switch to InMemory for quick testing)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// signalr
builder.Services.AddSignalR();

// cors
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClient", policy => {
        policy
            .WithOrigins("http://localhost:1420") // Tauri dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// repositories & services
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<GuildServer>, ServerRepository>();
builder.Services.AddScoped<IRepository<Channel>, ChannelRepository>();
builder.Services.AddScoped<IRepository<ThreadChannel>, ThreadRepository>();
builder.Services.AddScoped<IRepository<DmChannel>, DmChannelRepository>();
builder.Services.AddScoped<IRepository<GroupDmChannel>, GroupDmChannelRepository>();
builder.Services.AddScoped<IRepository<Message>, MessageRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<IServerService, ServerService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IThreadService, ThreadService>();
builder.Services.AddScoped<IDmChannelService, DmChannelService>();
builder.Services.AddScoped<IGroupDmChannelService, GroupDmChannelService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSingleton<IUserIdProvider, DbUserIdProvider>();

// background services
builder.Services.AddHostedService<PresenceCleanupService>();
builder.Services.AddHostedService<AttachmentCleanupService>();

// simple in-memory rate limiter config
builder.Services.Configure<RateLimitOptions>(configuration.GetSection("RateLimiting"));

// jwt auth
var jwtSection = configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new Exception("jwt key is not defined")));

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,

        IssuerSigningKey = signingKey,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidAudience = jwtSection.GetValue<string>("Audience")
    };
});

// add MemoryCache for rate limiter/state
builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    if (app.Environment.IsDevelopment()) {
        // clean upload directories
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        void CleanDirectory(string relativePath) {
            var fullPath = Path.Combine(env.ContentRootPath, relativePath);

            if (!Directory.Exists(fullPath))
                return;

            foreach (var file in Directory.GetFiles(fullPath))
                try { File.Delete(file); } catch { }

            foreach (var dir in Directory.GetDirectories(fullPath))
                try { Directory.Delete(dir, recursive: true); } catch { }
        }

        CleanDirectory("Uploads");
        CleanDirectory("Avatars");
        CleanDirectory("Banners");
        CleanDirectory("Fonts");
        CleanDirectory("Emojis");

        // auto register GOD user
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var user = await userService.RegisterAsync(new Server.DTOs.Input.RegisterDto {
            Email = "god@heaven.gov",
            Username = "GOD",
            Password = "string"
        });
        // create a testing server
        var serverService = scope.ServiceProvider.GetRequiredService<IServerService>();
        var server = await serverService.CreateServerAsync(new Server.DTOs.Input.ServerCreateDto {
            Name = "Testing Server",
            Description = "super cool testing server",
            Tags = ["test"],
            InviteUrls = ["test"]
        }, user.Id);
        var channelService = scope.ServiceProvider.GetRequiredService<IChannelService>();
        await channelService.CreateChannelAsync(new Server.DTOs.Input.ChannelCreateDto {
            Name = "general",
            Description = "lalala test description",
            Type = Server.Models.Enums.ChannelType.Text
        }, server.Id, user.Id);
        var alfred = await userService.RegisterAsync(new Server.DTOs.Input.RegisterDto {
            Email = "alfred@heaven.gov",
            Username = "alfred",
            Password = "string"
        });
        await serverService.JoinServerAsync(server.Id, alfred.Id);
    }
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowClient");
app.UseAuthentication();
app.UseAuthorization();

// rate limiting middleware
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();
app.MapHub<GatewayHub>("/gateway");

app.Run();