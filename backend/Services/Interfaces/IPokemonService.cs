using PokemonBattle.DTOs;

namespace PokemonBattle.Services.Interfaces {
    
    public interface IPokemonService {
        Task<List<PlayablePokemonDto>> GetAllPlayablePokemonAsync();
        Task<PlayablePokemonDto?> GetPlayablePokemonByNameAsync(string name);
        Task<List<PlayablePokemonDto>> GetPlayablePokemonByTypeAsync(string type);
        Task<List<string>> GetAvailableTypesAsync();
        Task<List<PlayablePokemonDto>> GetPokemonSortedByStatAsync(string stat);
        Task<bool> PokemonExistsAsync(string name);
        Task<int> GetPokemonCountAsync();
        Task<Dictionary<string, object>> GetPokemonStatisticsAsync();
    }
}
