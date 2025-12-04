using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    public class Snorlax : Pokemon {
        public Snorlax(string name="Snorlax", int level=30) : base(name, level) {
            Type = PokemonType.Normal;
            MaxHitPoint = 160 + (level * 8);
            CurrentHitPoint = MaxHitPoint;
            Attack = 110 + (level * 4);
            Defense = 65 + (level * 3);
            Speed = 30 + (level * 1);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new BodySlam(Attack);
            SpecialSkill = SpecialAttack.Name;
        }
    }
}