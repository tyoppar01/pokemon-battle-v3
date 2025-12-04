using PokemonBattle.Pokemons;

namespace PokemonBattle.Services {
    
    public class PokemonFactory {
        
        public static Pokemon CreatePokemon(string pokemonType, string name, int level) {
            // Use default name if not provided
            if (string.IsNullOrWhiteSpace(name)) {
                name = GetDefaultName(pokemonType);
            }

            // Ensure level is valid
            if (level < 1) level = 1;
            if (level > 100) level = 100;

            return pokemonType.ToLower() switch {
                "pikachu" or "electric" => new Pikachu(name, level),
                "charmander" or "fire" => new Charmander(name, level),
                "squirtle" or "water" => new Squirtle(name, level),
                "bulbasaur" or "grass" => new Bulbasaur(name, level),
                "gengar" or "ghost" => new Gengar(name, level),
                "mewtwo" or "psychic" => new Mewtwo(name, level),
                "snorlax" or "normal" => new Snorlax(name, level),
                "aron" or "steel" => new Aron(name, level),
                _ => throw new ArgumentException($"Unknown Pokemon type: {pokemonType}")
            };
        }

        private static string GetDefaultName(string pokemonType) {
            return pokemonType.ToLower() switch {
                "pikachu" or "electric" => "Pikachu",
                "charmander" or "fire" => "Charmander",
                "squirtle" or "water" => "Squirtle",
                "bulbasaur" or "grass" => "Bulbasaur",
                "gengar" or "ghost" => "Gengar",
                "mewtwo" or "psychic" => "Mewtwo",
                "snorlax" or "normal" => "Snorlax",
                "aron" or "steel" => "Aron",
                _ => "Pokemon"
            };
        }

        public static List<string> GetAvailablePokemonTypes() {
            return new List<string> {
                "Pikachu",
                "Charmander",
                "Squirtle",
                "Bulbasaur",
                "Gengar",
                "Mewtwo",
                "Snorlax",
                "Aron"
            };
        }
    }

}
