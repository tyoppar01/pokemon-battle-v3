namespace PokemonBattle.Attacks {
    
    public abstract class Attack {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }

        protected Attack(string name, int power, int accuracy) {
            Name = name;
            Power = power;
            Accuracy = accuracy;
        }

        protected Attack() {}

        public abstract void Execute();
    }

}