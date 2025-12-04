namespace PokemonBattle.DTOs {
    
    public class PlayablePokemonDto {
        public string Name { get; set; }
        public string Type { get; set; }
        public int BaseMaxHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpeed { get; set; }
        public string NormalAttack { get; set; }
        public string SpecialAttack { get; set; }
        public string Description { get; set; }
    }

}
