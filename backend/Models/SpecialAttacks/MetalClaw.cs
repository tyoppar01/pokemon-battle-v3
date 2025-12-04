namespace PokemonBattle.SpecialAttacks {
    
    public class MetalClaw : SpecialAttack {
        public MetalClaw(int basePower) : base("Metal Claw", basePower, 95, "Steel") {
        }

        public override void Execute() {
            // Steel type special attack - high power with chance to raise attack
        }
    }

}
