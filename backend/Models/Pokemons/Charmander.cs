using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    public class Charmander : Pokemon {
        public Charmander(string name="Charmander", int level=1) : base(name, level) {
            Type = PokemonType.Fire;
            MaxHitPoint = 39 + (level * 4);
            CurrentHitPoint = MaxHitPoint;
            Attack = 52 + (level * 4);
            Defense = 43 + (level * 2);
            Speed = 65 + (level * 3);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new Ember(Attack);
            SpecialSkill = SpecialAttack.Name;
        }
    }
    
}