using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    public class Mewtwo : Pokemon {
        public Mewtwo(string name="Mewtwo", int level=99) : base(name, level) {
            Type = PokemonType.Psychic;
            MaxHitPoint = 106 + (level * 5);
            CurrentHitPoint = MaxHitPoint;
            Attack = 110 + (level * 5);
            Defense = 90 + (level * 4);
            Speed = 130 + (level * 6);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new Psychic(Attack);
        }
    }
    
}