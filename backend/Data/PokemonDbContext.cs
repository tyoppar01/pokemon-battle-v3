using Microsoft.EntityFrameworkCore;
using PokemonBattle.Data.Entities;
using PokemonBattle.Services;

namespace PokemonBattle.Data {
    
    /**
        * Pokemon Database Context
        * Manages database access for users and Pokemon
    */
    public class PokemonDbContext : DbContext {

        /**
            * Database Sets
        */  
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PokemonEntity> Pokemon { get; set; }
        public DbSet<PlayablePokemonEntity> PlayablePokemon { get; set; }

        /**
            * Constructor
        */
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
            // No default users - users must be created through the application
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
