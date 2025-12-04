using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    public class Pikachu : Pokemon {
        public Pikachu(string name="Pikachu", int level=5) : base(name, level) {
            Type = PokemonType.Electric;
            MaxHitPoint = 35 + (level * 4);
            CurrentHitPoint = MaxHitPoint;
            Attack = 55 + (level * 3);
            Defense = 40 + (level * 2);
            Speed = 90 + (level * 5);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new ThunderShock(Attack);
            SpecialSkill = SpecialAttack.Name;
        }
    }
    
}