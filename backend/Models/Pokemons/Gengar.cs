using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {
    public class Gengar : Pokemon {
        public Gengar(string name="Gengar", int level=1) : base(name, level) {
            Type = PokemonType.Ghost;
            MaxHitPoint = 60 + (level * 4);
            CurrentHitPoint = MaxHitPoint;
            Attack = 65 + (level * 4);
            Defense = 60 + (level * 2);
            Speed = 110 + (level * 6);
            NormalAttack = new Tackle(Attack);
            SpecialAttack = new ShadowBall(Attack);
            SpecialSkill = SpecialAttack.Name;
        }
    }
    
}