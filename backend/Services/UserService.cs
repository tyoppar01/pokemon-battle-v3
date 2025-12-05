using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PokemonBattle.Characters;
using PokemonBattle.Pokemons;
using PokemonBattle.Data;
using PokemonBattle.Data.Entities;
using PokemonBattle.Services.Interfaces;

namespace PokemonBattle.Services {
    
    public class UserService : IUserService {
        private readonly PokemonDbContext _context;
        private static int userIdCounter = 1;

        public UserService(PokemonDbContext context) {
            _context = context;
        }

        public string CreateUser(string name, string gender = "Unknown") {
            string userId = $"user_{userIdCounter++}";
            
            var userEntity = new UserEntity {
                Id = userId,
                Name = name,
                Gender = gender,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(userEntity);
            _context.SaveChanges();
            
            return userId;
        }

        public Character GetUser(string userId) {
            var userEntity = _context.Users
                .Include(u => u.Pokemon)
                .FirstOrDefault(u => u.Id == userId);
            
            if (userEntity == null) return null;

            var character = new Character {
                Name = userEntity.Name,
                Gender = userEntity.Gender
            };

            // Convert Pokemon entities to Pokemon objects
            foreach (var pokemonEntity in userEntity.Pokemon) {
                var pokemon = PokemonFactory.CreatePokemon(
                    pokemonEntity.Type,
                    pokemonEntity.Name,
                    pokemonEntity.Level
                );
                
                // Restore current HP
                pokemon.CurrentHitPoint = pokemonEntity.CurrentHitPoint;
                character.AddPokemon(pokemon);
            }

            return character;
        }

        public List<KeyValuePair<string, Character>> GetAllUsers() {
            var users = _context.Users
                .Include(u => u.Pokemon)
                .ToList();

            var result = new List<KeyValuePair<string, Character>>();
            
            foreach (var userEntity in users) {
                var character = new Character {
                    Name = userEntity.Name,
                    Gender = userEntity.Gender
                };

                foreach (var pokemonEntity in userEntity.Pokemon) {
                    var pokemon = PokemonFactory.CreatePokemon(
                        pokemonEntity.Type,
                        pokemonEntity.Name,
                        pokemonEntity.Level
                    );
                    pokemon.CurrentHitPoint = pokemonEntity.CurrentHitPoint;
                    character.AddPokemon(pokemon);
                }

                result.Add(new KeyValuePair<string, Character>(userEntity.Id, character));
            }

            return result;
        }

        public bool DeleteUser(string userId) {
            var userEntity = _context.Users.Find(userId);
            if (userEntity == null) return false;

            _context.Users.Remove(userEntity);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateUser(string userId, string name, string gender) {
            var userEntity = _context.Users.Find(userId);
            if (userEntity == null) return false;
            
            userEntity.Name = name;
            userEntity.Gender = gender;
            _context.SaveChanges();
            return true;
        }

        // Pokemon Team Management
        public bool AddPokemonToUser(string userId, Pokemon pokemon) {
            var userEntity = _context.Users.Find(userId);
            if (userEntity == null) return false;

            var pokemonEntity = new PokemonEntity {
                UserId = userId,
                Name = pokemon.Name,
                Type = pokemon.Type.ToString(),
                MaxHitPoint = pokemon.MaxHitPoint,
                CurrentHitPoint = pokemon.CurrentHitPoint,
                Attack = pokemon.Attack,
                Defense = pokemon.Defense,
                Level = pokemon.Level,
                Speed = pokemon.Speed,
                SpecialSkill = pokemon.SpecialSkill,
                CreatedAt = DateTime.UtcNow
            };

            _context.Pokemon.Add(pokemonEntity);
            _context.SaveChanges();
            return true;
        }

        public bool RemovePokemonFromUser(string userId, int pokemonIndex) {
            var userPokemon = _context.Pokemon
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Id)
                .ToList();

            if (pokemonIndex < 0 || pokemonIndex >= userPokemon.Count) return false;
            
            _context.Pokemon.Remove(userPokemon[pokemonIndex]);
            _context.SaveChanges();
            return true;
        }

        public List<Pokemon> GetUserPokemon(string userId) {
            var pokemonEntities = _context.Pokemon
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Id)
                .ToList();

            var pokemonList = new List<Pokemon>();
            foreach (var entity in pokemonEntities) {
                var pokemon = PokemonFactory.CreatePokemon(
                    entity.Type,
                    entity.Name,
                    entity.Level
                );
                pokemon.CurrentHitPoint = entity.CurrentHitPoint;
                pokemonList.Add(pokemon);
            }

            return pokemonList;
        }
    }

}
