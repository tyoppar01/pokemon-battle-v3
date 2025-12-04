namespace PokemonBattle.SpecialAttacks {
    
    public class ThunderShock : SpecialAttack {
        public ThunderShock(int basePower) : base("Thunder Shock", basePower, 100, "Electric") {
        }

        public override void Execute() {
            // Electric type special attack - may cause paralysis
        }
    }

}
