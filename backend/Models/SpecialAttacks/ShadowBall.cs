namespace PokemonBattle.SpecialAttacks {
    
    public class ShadowBall : SpecialAttack {
        public ShadowBall(int basePower) : base("Shadow Ball", basePower, 100, "Ghost") {
        }

        public override void Execute() {
            // Ghost type special attack - may lower special defense
        }
    }

}
