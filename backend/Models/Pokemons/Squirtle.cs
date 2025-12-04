using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    
    public class Squirtle : Pokemon {
        public Squirtle(string name="Squirtle", int level=5) : base(name, level) {
            Type = PokemonType.Water;
            MaxHitPoint = 44 + (level * 5);
            CurrentHitPoint = MaxHitPoint;
            Attack = 48 + (level * 3);
            Defense = 65 + (level * 3);
            Speed = 43 + (level * 2);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new WaterGun(Attack);
            SpecialSkill = SpecialAttack.Name;
        }
    }

}