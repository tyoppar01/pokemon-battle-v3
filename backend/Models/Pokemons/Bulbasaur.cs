using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    public class Bulbasaur : Pokemon {
        public Bulbasaur(string name="Bulbasaur", int level=1) : base(name, level) {
            Type = PokemonType.Grass;
            MaxHitPoint = 45 + (level * 5);
            CurrentHitPoint = MaxHitPoint;
            Attack = 49 + (level * 3);
            Defense = 49 + (level * 3);
            Speed = 45 + (level * 3);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new VineWhip(Attack);
        }
    }
    
}