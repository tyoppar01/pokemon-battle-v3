using PokemonBattle.Enums;

namespace PokemonBattle.Helpers {
    public static class TypeEffectivenessHelper {
        
        public static TypeEffectiveness GetEffectiveness(PokemonType attackerType, PokemonType defenderType) {
            // Super Effective matchups
            if (attackerType == PokemonType.Fire && defenderType == PokemonType.Grass) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Fire && defenderType == PokemonType.Steel) return TypeEffectiveness.SuperEffective;
            
            if (attackerType == PokemonType.Water && defenderType == PokemonType.Fire) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Water && defenderType == PokemonType.Ground) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Water && defenderType == PokemonType.Rock) return TypeEffectiveness.SuperEffective;
            
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Water) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Ground) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Rock) return TypeEffectiveness.SuperEffective;
            
            if (attackerType == PokemonType.Electric && defenderType == PokemonType.Water) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Electric && defenderType == PokemonType.Flying) return TypeEffectiveness.SuperEffective;
            
            if (attackerType == PokemonType.Psychic && defenderType == PokemonType.Fighting) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Psychic && defenderType == PokemonType.Poison) return TypeEffectiveness.SuperEffective;
            
            if (attackerType == PokemonType.Ghost && defenderType == PokemonType.Psychic) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Ghost && defenderType == PokemonType.Ghost) return TypeEffectiveness.SuperEffective;
            
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Rock) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Ice) return TypeEffectiveness.SuperEffective;
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Fairy) return TypeEffectiveness.SuperEffective;
            
            // Not Very Effective matchups
            if (attackerType == PokemonType.Fire && defenderType == PokemonType.Water) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Fire && defenderType == PokemonType.Fire) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Fire && defenderType == PokemonType.Rock) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Water && defenderType == PokemonType.Water) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Water && defenderType == PokemonType.Grass) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Fire) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Grass) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Flying) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Grass && defenderType == PokemonType.Steel) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Electric && defenderType == PokemonType.Electric) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Electric && defenderType == PokemonType.Grass) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Psychic && defenderType == PokemonType.Psychic) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Psychic && defenderType == PokemonType.Steel) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Ghost && defenderType == PokemonType.Dark) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Fire) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Water) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Electric) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Steel && defenderType == PokemonType.Steel) return TypeEffectiveness.NotVeryEffective;
            
            if (attackerType == PokemonType.Normal && defenderType == PokemonType.Rock) return TypeEffectiveness.NotVeryEffective;
            if (attackerType == PokemonType.Normal && defenderType == PokemonType.Steel) return TypeEffectiveness.NotVeryEffective;
            
            // Default: Normal effectiveness
            return TypeEffectiveness.Normal;
        }
        
        public static double GetMultiplier(TypeEffectiveness effectiveness) {
            return effectiveness switch {
                TypeEffectiveness.SuperEffective => 2.0,
                TypeEffectiveness.NotVeryEffective => 0.5,
                _ => 1.0
            };
        }
        
        public static string GetEffectivenessMessage(TypeEffectiveness effectiveness) {
            return effectiveness switch {
                TypeEffectiveness.SuperEffective => "It's super effective!",
                TypeEffectiveness.NotVeryEffective => "It's not very effective...",
                _ => ""
            };
        }
    }
}
