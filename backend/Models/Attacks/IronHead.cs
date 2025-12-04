namespace PokemonBattle.Attacks {
    public class IronHead : Attack {
        public IronHead(int power) : base("Iron Head", power, 60) { }

        public override void Execute() {
            Console.WriteLine($"{Name} attack executed with power {Power}!");
        }

    }
}