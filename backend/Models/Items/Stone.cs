namespace PokemonBattle.Items {

    public class Stone : Item {
        public string EvolutionType { get; set; }

        public Stone(string name, string description, string evolutionType) : base(name, description) {
            EvolutionType = evolutionType;
        }

        public Stone() {
        }
    }

}
