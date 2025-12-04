using System.Collections.Generic;
using PokemonBattle.Characters;

namespace PokemonBattle.DTOs {
    
    public class UserResponse {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int PokemonCount { get; set; }
        public List<PokemonDto> Pokemon { get; set; }

        public static UserResponse FromCharacter(Character character, string id) {
            return new UserResponse {
                Id = id,
                Name = character.Name,
                Gender = character.Gender,
                PokemonCount = character.PokePockets.Count,
                Pokemon = character.PokePockets.Select(p => PokemonDto.FromPokemon(p)).ToList()
            };
        }
    }

    public class PokemonDto {
        public string Name { get; set; }
        public string Type { get; set; }
        public int MaxHitPoint { get; set; }
        public int CurrentHitPoint { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Level { get; set; }
        public int Speed { get; set; }
        public string SpecialSkill { get; set; }

        public static PokemonDto FromPokemon(Pokemon pokemon) {
            return new PokemonDto {
                Name = pokemon.Name,
                Type = pokemon.Type.ToString(),
                MaxHitPoint = pokemon.MaxHitPoint,
                CurrentHitPoint = pokemon.CurrentHitPoint,
                Attack = pokemon.Attack,
                Defense = pokemon.Defense,
                Level = pokemon.Level,
                Speed = pokemon.Speed,
                SpecialSkill = pokemon.SpecialSkill
            };
        }
    }

}
