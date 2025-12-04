namespace PokemonBattle.Items {
    public class Pokeball : Item {
        public int CatchRate { get; set; }

        public Pokeball(string name, string description, int catchRate) : base(name, description) { 
            CatchRate = catchRate; 
        }

        public Pokeball() {}
    }
}
