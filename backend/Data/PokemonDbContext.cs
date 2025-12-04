using Microsoft.EntityFrameworkCore;
using PokemonBattle.Data.Entities;
using PokemonBattle.Services;

namespace PokemonBattle.Data {
    
    public class PokemonDbContext : DbContext {
        
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PokemonEntity> Pokemon { get; set; }
        public DbSet<PlayablePokemonEntity> PlayablePokemon { get; set; }

        public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<UserEntity>()
                .HasMany(u => u.Pokemon)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed default Pokemon data
            SeedDefaultPokemon(modelBuilder);
            
            // Seed playable Pokemon templates
            SeedPlayablePokemon(modelBuilder);
        }

        private void SeedDefaultPokemon(ModelBuilder modelBuilder) {
            // Create default users
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity {
                    Id = "default_user_1",
                    Name = "Ash",
                    Gender = "Male",
                    CreatedAt = DateTime.UtcNow
                },
                new UserEntity {
                    Id = "default_user_2",
                    Name = "Gary",
                    Gender = "Male",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Use Pokemon models to generate seed data
            var pikachu = PokemonFactory.CreatePokemon("Pikachu", "Pikachu", 5);
            var charmander = PokemonFactory.CreatePokemon("Charmander", "Charmander", 5);
            var bulbasaur = PokemonFactory.CreatePokemon("Bulbasaur", "Bulbasaur", 5);
            var squirtle = PokemonFactory.CreatePokemon("Squirtle", "Squirtle", 5);

            // Seed default Pokemon for user 1 (Ash)
            modelBuilder.Entity<PokemonEntity>().HasData(
                new PokemonEntity {
                    Id = 1,
                    UserId = "default_user_1",
                    Name = pikachu.Name,
                    Type = pikachu.Type.ToString(),
                    MaxHitPoint = pikachu.MaxHitPoint,
                    CurrentHitPoint = pikachu.CurrentHitPoint,
                    Attack = pikachu.Attack,
                    Defense = pikachu.Defense,
                    Level = pikachu.Level,
                    Speed = pikachu.Speed,
                    SpecialSkill = pikachu.SpecialAttack?.Name ?? "Unknown",
                    CreatedAt = DateTime.UtcNow
                },
                new PokemonEntity {
                    Id = 2,
                    UserId = "default_user_1",
                    Name = charmander.Name,
                    Type = charmander.Type.ToString(),
                    MaxHitPoint = charmander.MaxHitPoint,
                    CurrentHitPoint = charmander.CurrentHitPoint,
                    Attack = charmander.Attack,
                    Defense = charmander.Defense,
                    Level = charmander.Level,
                    Speed = charmander.Speed,
                    SpecialSkill = charmander.SpecialAttack?.Name ?? "Unknown",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed default Pokemon for user 2 (Gary)
            modelBuilder.Entity<PokemonEntity>().HasData(
                new PokemonEntity {
                    Id = 3,
                    UserId = "default_user_2",
                    Name = bulbasaur.Name,
                    Type = bulbasaur.Type.ToString(),
                    MaxHitPoint = bulbasaur.MaxHitPoint,
                    CurrentHitPoint = bulbasaur.CurrentHitPoint,
                    Attack = bulbasaur.Attack,
                    Defense = bulbasaur.Defense,
                    Level = bulbasaur.Level,
                    Speed = bulbasaur.Speed,
                    SpecialSkill = bulbasaur.SpecialAttack?.Name ?? "Unknown",
                    CreatedAt = DateTime.UtcNow
                },
                new PokemonEntity {
                    Id = 4,
                    UserId = "default_user_2",
                    Name = squirtle.Name,
                    Type = squirtle.Type.ToString(),
                    MaxHitPoint = squirtle.MaxHitPoint,
                    CurrentHitPoint = squirtle.CurrentHitPoint,
                    Attack = squirtle.Attack,
                    Defense = squirtle.Defense,
                    Level = squirtle.Level,
                    Speed = squirtle.Speed,
                    SpecialSkill = squirtle.SpecialAttack?.Name ?? "Unknown",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
        
        private void SeedPlayablePokemon(ModelBuilder modelBuilder) {
            var createdAt = DateTime.UtcNow;
            var pokemonTypes = PokemonFactory.GetAvailablePokemonTypes();
            var descriptions = new Dictionary<string, string> {
                { "Pikachu", "An Electric-type Pokemon known for its incredible speed and powerful electric attacks." },
                { "Charmander", "A Fire-type Pokemon with a flame on its tail that indicates its emotional and physical state." },
                { "Squirtle", "A Water-type Pokemon with a tough shell that provides excellent defense." },
                { "Bulbasaur", "A Grass-type Pokemon that has a plant bulb on its back, growing as it gains experience." },
                { "Gengar", "A Ghost-type Pokemon that lurks in shadows and has devastating special attacks." },
                { "Mewtwo", "A legendary Psychic-type Pokemon created through genetic manipulation, extremely powerful." },
                { "Snorlax", "A Normal-type Pokemon known for its massive HP and tendency to sleep." },
                { "Aron", "A Steel-type Pokemon covered in sturdy armor, boasting exceptional defense." }
            };

            var playablePokemonList = new List<PlayablePokemonEntity>();
            int id = 1;

            foreach (var pokemonName in pokemonTypes) {
                var pokemon = PokemonFactory.CreatePokemon(pokemonName, pokemonName, 1);
                
                playablePokemonList.Add(new PlayablePokemonEntity {
                    Id = id++,
                    Name = pokemon.Name,
                    Type = pokemon.Type.ToString(),
                    BaseMaxHP = pokemon.MaxHitPoint,
                    BaseAttack = pokemon.Attack,
                    BaseDefense = pokemon.Defense,
                    BaseSpeed = pokemon.Speed,
                    NormalAttack = pokemon.NormalAttack?.Name ?? "Tackle",
                    SpecialAttack = pokemon.SpecialAttack?.Name ?? "Unknown",
                    Description = descriptions.GetValueOrDefault(pokemonName, "A mysterious Pokemon."),
                    IsAvailable = true,
                    CreatedAt = createdAt
                });
            }

            modelBuilder.Entity<PlayablePokemonEntity>().HasData(playablePokemonList.ToArray());
        }
    }

}
