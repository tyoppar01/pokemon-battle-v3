namespace PokemonBattle.Attacks {
    
    public class Tackle : Attack {
        public Tackle(int power) : base("Tackle", power, 100) {
        }

        public override void Execute() {
            Console.WriteLine($"{Name} attack executed with power {Power}!");
        }
    }

}