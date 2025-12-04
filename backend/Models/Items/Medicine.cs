namespace PokemonBattle.Items {
    public class Medicine : Item {
        public int HealAmount { get; set; }

        public Medicine(string name, string description, int healAmount) : base(name, description) {
            HealAmount = healAmount;
        }

        public Medicine() {}
    }
}
