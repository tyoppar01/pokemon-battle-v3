namespace PokemonBattle.SpecialAttacks {
    
    public class Ember : SpecialAttack {
        public Ember(int basePower) : base("Ember", basePower, 100, "Fire") {
        }

        public override void Execute() {
            // Fire type special attack - may cause burn
        }
    }

}
