using PokemonBattle.Data;
using PokemonBattle.Data.Entities;
using PokemonBattle.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using PokemonBattle.Pokemons;

namespace PokemonBattle.Services {
    
    public class PokemonService {
        private readonly PokemonDbContext _context;
        private static readonly Dictionary<string, string> _pokemonDescriptions = new() {
            { "Pikachu", "An Electric-type Pokemon known for its incredible speed and powerful electric attacks." },
            { "Charmander", "A Fire-type Pokemon with a flame on its tail that indicates its emotional and physical state." },
            { "Squirtle", "A Water-type Pokemon with a tough shell that provides excellent defense." },
            { "Bulbasaur", "A Grass-type Pokemon that has a plant bulb on its back, growing as it gains experience." },
            { "Gengar", "A Ghost-type Pokemon that lurks in shadows and has devastating special attacks." },
            { "Mewtwo", "A legendary Psychic-type Pokemon created through genetic manipulation, extremely powerful." },
            { "Snorlax", "A Normal-type Pokemon known for its massive HP and tendency to sleep." },
            { "Aron", "A Steel-type Pokemon covered in sturdy armor, boasting exceptional defense." }
        };

        public PokemonService(PokemonDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Get all playable Pokemon - generates from Pokemon models dynamically
        /// </summary>
        public async Task<List<PlayablePokemonDto>> GetAllPlayablePokemonAsync() {
            // Check if we have data in database, if not generate from models
            var dbPokemon = await _context.PlayablePokemon
                .Where(p => p.IsAvailable)
                .ToListAsync();

            if (dbPokemon.Any()) {
                return dbPokemon.Select(p => MapToDto(p)).ToList();
            }

            // Generate from Pokemon models using PokemonFactory
            return GenerateFromModels();
        }

        /// <summary>
        /// Generate Pokemon data from model classes
        /// </summary>
        private List<PlayablePokemonDto> GenerateFromModels() {
            var pokemonTypes = PokemonFactory.GetAvailablePokemonTypes();
            var result = new List<PlayablePokemonDto>();

            foreach (var pokemonName in pokemonTypes) {
                var pokemon = PokemonFactory.CreatePokemon(pokemonName, pokemonName, 1);
                
                result.Add(new PlayablePokemonDto {
                    Name = pokemonName,
                    Type = pokemon.Type.ToString(),
                    BaseMaxHP = pokemon.MaxHitPoint,
                    BaseAttack = pokemon.Attack,
                    BaseDefense = pokemon.Defense,
                    BaseSpeed = pokemon.Speed,
                    NormalAttack = pokemon.NormalAttack?.Name ?? "Tackle",
                    SpecialAttack = pokemon.SpecialAttack?.Name ?? "Unknown",
                    Description = _pokemonDescriptions.GetValueOrDefault(pokemonName, "A mysterious Pokemon.")
                });
            }

            return result;
        }

        /// <summary>
        /// Get a specific playable Pokemon by name
        /// </summary>
        public async Task<PlayablePokemonDto?> GetPlayablePokemonByNameAsync(string name) {
            // Try from database first
            var dbPokemon = await _context.PlayablePokemon
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower() && p.IsAvailable);

            if (dbPokemon != null) {
                return MapToDto(dbPokemon);
            }

            // Generate from model
            var pokemonTypes = PokemonFactory.GetAvailablePokemonTypes();
            var matchingType = pokemonTypes.FirstOrDefault(t => 
                t.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (matchingType == null) {
                return null;
            }

            var pokemon = PokemonFactory.CreatePokemon(matchingType, matchingType, 1);
            
            return new PlayablePokemonDto {
                Name = matchingType,
                Type = pokemon.Type.ToString(),
                BaseMaxHP = pokemon.MaxHitPoint,
                BaseAttack = pokemon.Attack,
                BaseDefense = pokemon.Defense,
                BaseSpeed = pokemon.Speed,
                NormalAttack = pokemon.NormalAttack?.Name ?? "Tackle",
                SpecialAttack = pokemon.SpecialAttack?.Name ?? "Unknown",
                Description = _pokemonDescriptions.GetValueOrDefault(matchingType, "A mysterious Pokemon.")
            };
        }

        /// <summary>
        /// Get playable Pokemon by type
        /// </summary>
        public async Task<List<PlayablePokemonDto>> GetPlayablePokemonByTypeAsync(string type) {
            var allPokemon = await GetAllPlayablePokemonAsync();
            return allPokemon
                .Where(p => p.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Get all Pokemon types available
        /// </summary>
        public async Task<List<string>> GetAvailableTypesAsync() {
            var allPokemon = await GetAllPlayablePokemonAsync();
            return allPokemon
                .Select(p => p.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        /// <summary>
        /// Get Pokemon sorted by a specific stat
        /// </summary>
        public async Task<List<PlayablePokemonDto>> GetPokemonSortedByStatAsync(string stat) {
            var allPokemon = await GetAllPlayablePokemonAsync();

            return stat.ToLower() switch {
                "hp" => allPokemon.OrderByDescending(p => p.BaseMaxHP).ToList(),
                "attack" => allPokemon.OrderByDescending(p => p.BaseAttack).ToList(),
                "defense" => allPokemon.OrderByDescending(p => p.BaseDefense).ToList(),
                "speed" => allPokemon.OrderByDescending(p => p.BaseSpeed).ToList(),
                _ => allPokemon.OrderBy(p => p.Name).ToList()
            };
        }

        /// <summary>
        /// Check if a Pokemon exists and is available
        /// </summary>
        public async Task<bool> PokemonExistsAsync(string name) {
            var pokemon = await GetPlayablePokemonByNameAsync(name);
            return pokemon != null;
        }

        /// <summary>
        /// Get Pokemon count
        /// </summary>
        public async Task<int> GetPokemonCountAsync() {
            var allPokemon = await GetAllPlayablePokemonAsync();
            return allPokemon.Count;
        }

        /// <summary>
        /// Get Pokemon statistics summary
        /// </summary>
        public async Task<Dictionary<string, object>> GetPokemonStatisticsAsync() {
            var pokemon = await GetAllPlayablePokemonAsync();

            var typeGroups = pokemon.GroupBy(p => p.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            return new Dictionary<string, object> {
                { "totalCount", pokemon.Count },
                { "typeDistribution", typeGroups },
                { "averageHP", pokemon.Average(p => p.BaseMaxHP) },
                { "averageAttack", pokemon.Average(p => p.BaseAttack) },
                { "averageDefense", pokemon.Average(p => p.BaseDefense) },
                { "averageSpeed", pokemon.Average(p => p.BaseSpeed) },
                { "strongestPokemon", pokemon.OrderByDescending(p => p.BaseAttack).First().Name },
                { "fastestPokemon", pokemon.OrderByDescending(p => p.BaseSpeed).First().Name },
                { "tankiestPokemon", pokemon.OrderByDescending(p => p.BaseDefense).First().Name }
            };
        }

        /// <summary>
        /// Map entity to DTO (for database records if they exist)
        /// </summary>
        private PlayablePokemonDto MapToDto(PlayablePokemonEntity entity) {
            return new PlayablePokemonDto {
                Name = entity.Name,
                Type = entity.Type,
                BaseMaxHP = entity.BaseMaxHP,
                BaseAttack = entity.BaseAttack,
                BaseDefense = entity.BaseDefense,
                BaseSpeed = entity.BaseSpeed,
                NormalAttack = entity.NormalAttack,
                SpecialAttack = entity.SpecialAttack,
                Description = entity.Description
            };
        }
    }

}
