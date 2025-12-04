namespace PokemonBattle.SpecialAttacks {
    
    public abstract class SpecialAttack {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public string Type { get; set; }

        protected SpecialAttack(string name, int power, int accuracy, string type) {
            Name = name;
            Power = power;
            Accuracy = accuracy;
            Type = type;
        }

        protected SpecialAttack() {}

        public abstract void Execute();
    }

}
