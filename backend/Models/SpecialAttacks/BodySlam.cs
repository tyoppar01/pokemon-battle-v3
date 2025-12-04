namespace PokemonBattle.SpecialAttacks {
    
    public class BodySlam : SpecialAttack {
        public BodySlam(int basePower) : base("Body Slam", basePower, 100, "Normal") {
        }

        public override void Execute() {
            // Normal type special attack - heavy body slam, may cause paralysis
        }
    }

}
