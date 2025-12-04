namespace PokemonBattle.SpecialAttacks {
    
    public class Psychic : SpecialAttack {
        public Psychic(int basePower) : base("Psychic", basePower, 100, "Psychic") {
        }

        public override void Execute() {
            // Psychic type special attack - powerful psychic energy
        }
    }

}
