namespace PokemonBattle.SpecialAttacks {
    
    public class VineWhip : SpecialAttack {
        public VineWhip(int basePower) : base("Vine Whip", basePower, 100, "Grass") {
        }

        public override void Execute() {
            // Grass type special attack - whipping vines
        }
    }

}
