using PokemonBattle.Attacks;
using PokemonBattle.SpecialAttacks;
using PokemonBattle.Enums;

/**
 * Represents a Pokemon entity with battle and capture attributes
 * 
 */
namespace PokemonBattle {
    
    public abstract class Pokemon {
        public string Name { get; set; }

        public int MaxHitPoint { get; set; }

        public int CurrentHitPoint { get; set; }

        public int Attack { get; set; }

        public int Level { get; set; }

        public int Defense { get; set; }

        public int Speed { get; set; }

        public PokemonType Type { get; set; }

        public Attack NormalAttack { get; set; }

        public SpecialAttack SpecialAttack { get; set; }

        public string ElementaryAttack { get; set; }

        public string SpecialSkill { get; set; }

        public string Rarity { get; set; }

        public bool Catchable { get; set; }

        public bool OwnedByPlayer { get; set; }

        public Pokemon(int maxHitPoint, int attack, PokemonType type, bool ownedByPlayer) {
            MaxHitPoint = maxHitPoint;
            CurrentHitPoint = maxHitPoint;
            Attack = attack;
            Type = type;
            OwnedByPlayer = ownedByPlayer;
        }

        public Pokemon(string name, int level) {
            Name = name;
            Level = level;
        }

        public void Heal(int amount) {
            CurrentHitPoint = Math.Min(CurrentHitPoint + amount, MaxHitPoint);
        }

        public void FullHeal() {
            CurrentHitPoint = MaxHitPoint;
        }

        public bool IsFainted() {
            return CurrentHitPoint <= 0;
        }

    }
}
