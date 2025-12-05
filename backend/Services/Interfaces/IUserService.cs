using PokemonBattle.Characters;
using PokemonBattle.Pokemons;

namespace PokemonBattle.Services.Interfaces {
    
    public interface IUserService {
        string CreateUser(string name, string gender = "Unknown");
        Character GetUser(string userId);
        List<KeyValuePair<string, Character>> GetAllUsers();
        bool DeleteUser(string userId);
        bool UpdateUser(string userId, string name, string gender);
        bool AddPokemonToUser(string userId, Pokemon pokemon);
        bool RemovePokemonFromUser(string userId, int pokemonIndex);
        List<Pokemon> GetUserPokemon(string userId);
    }
}
