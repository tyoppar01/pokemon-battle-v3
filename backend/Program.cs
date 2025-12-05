using PokemonBattle.Services;
using PokemonBattle.Services.Interfaces;
using PokemonBattle.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure SQLite Database
var databasePath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? "Databases/pokemon_battle.db";
builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PokemonBattleAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PokemonBattleClient";
var jwtExpiryMinutes = int.Parse(builder.Configuration["Jwt:ExpiryMinutes"] ?? "60");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Register custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddSingleton(new JwtService(jwtKey, jwtIssuer, jwtAudience, jwtExpiryMinutes));

// Configure CORS for frontend
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS")?.Split(',') 
    ?? new[] { "http://localhost:8080" };

builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure Databases directory exists
var dbDirectory = Path.GetDirectoryName(databasePath);
if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory)) {
    Directory.CreateDirectory(dbDirectory);
    Console.WriteLine($"Created directory: {dbDirectory}");
}

// Initialize database and seed data
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<PokemonDbContext>();
    Console.WriteLine("Ensuring database is created...");
    db.Database.EnsureCreated();
    Console.WriteLine("Database ready!");
}

// Configure the HTTP request pipeline
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();