namespace PokemonBattle.SpecialAttacks {
    
    public class WaterGun : SpecialAttack {
        public WaterGun(int basePower) : base("Water Gun", basePower, 100, "Water") {
        }

        public override void Execute() {
            // Water type special attack - reliable water move
        }
    }

}
