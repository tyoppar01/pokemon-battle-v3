using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

namespace PokemonBattle.Pokemons {

    public class Aron : Pokemon {
        public Aron(string name="Aron", int level=1) : base(name, level) {
            Type = PokemonType.Steel;
            MaxHitPoint = 50 + (level * 5);
            CurrentHitPoint = MaxHitPoint;
            Attack = 70 + (level * 3);
            Defense = 100 + (level * 4);
            Speed = 30 + (level * 2);
            NormalAttack = new IronHead(Attack);
            SpecialAttack = new MetalClaw(Attack);
        }
    }
    
}