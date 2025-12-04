namespace PokemonBattle.DTOs {
    
    public class AddPokemonRequest {
        public string PokemonType { get; set; }
        public string Name { get; set; }
        public int Level { get; set; } = 1;
    }

}
