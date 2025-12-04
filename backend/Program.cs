using PokemonBattle.Services;
using PokemonBattle.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite Database
builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseSqlite("Data Source=Databases/pokemon_battle.db"));

// Register custom services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PokemonService>();

// Configure CORS for frontend
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins(
            "http://localhost:8000", 
            "http://127.0.0.1:8000", 
            "http://localhost:5500", 
            "http://127.0.0.1:5500",
            "http://localhost:5050",
            "http://127.0.0.1:5050",
            "http://localhost:8080",
            "http://127.0.0.1:8080"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure Databases directory exists
if (!Directory.Exists("Databases")) {
    Directory.CreateDirectory("Databases");
    Console.WriteLine("Created Databases directory");
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
