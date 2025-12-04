using PokemonBattle.Services;
using PokemonBattle.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite Database
var databasePath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? "Databases/pokemon_battle.db";
builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

// Register custom services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PokemonService>();

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
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();