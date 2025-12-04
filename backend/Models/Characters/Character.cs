using System.Collections.Generic;
using PokemonBattle;
using PokemonBattle.Inventories;
using PokemonBattle.Items;

namespace PokemonBattle.Characters {
    
    public class Character {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string SpecialSkill { get; set; }
        public List<Pokemon> PokePockets { get; set; }
        public Inventory<Item> Inventory { get; set; }

        public Character(string name, List<Pokemon> pokeTeam) {
            Name = name;
            PokePockets = pokeTeam;
            Inventory = new Inventory<Item>();
        }

        public Character() {
            Name = "PLAYER 1";
            Inventory = new Inventory<Item>();
            PokePockets = new List<Pokemon>();
        }

        public void AddPokemon(Pokemon pokemon) {
            PokePockets.Add(pokemon);
        }

        public bool RemovePokemon(Pokemon pokemon) {
            return PokePockets.Remove(pokemon);
        }
    }

}
