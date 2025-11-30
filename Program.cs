using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    options.UseSqlite($"Data Source=./HarmonicDb.sqlite"));
//    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<IServerService, ServerService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IThreadService, ThreadService>();
builder.Services.AddScoped<IDmChannelService, DmChannelService>();
builder.Services.AddScoped<IGroupDmChannelService, GroupDmChannelService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// background services
builder.Services.AddHostedService<PresenceCleanupService>();

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

    options.Events = new JwtBearerEvents {
        /*OnMessageReceived = context => {
            var authToken = context.Request.Headers.Authorization.FirstOrDefault();
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(authToken) && path.StartsWithSegments("/ws"))
                context.Token = authToken;

            return Task.CompletedTask;
        }*/
    };
});

// add MemoryCache for rate limiter/state
builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    //db.Database.Migrate();
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowClient");

// rate limiting middleware
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();
app.MapHub<GatewayHub>("/ws");

app.Run();