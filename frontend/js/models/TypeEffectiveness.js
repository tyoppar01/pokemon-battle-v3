/**
 * Type Effectiveness System
 * Handles Pokemon type matchup calculations
 */
export class TypeEffectiveness {
    /**
     * Get damage multiplier based on type matchup
     * @param {string} attackerType - Attacking Pokemon's type
     * @param {string} defenderType - Defending Pokemon's type
     * @returns {number} Damage multiplier (0, 0.5, 1.0, or 2.0)
     */
    static getMultiplier(attackerType, defenderType) {
        const effectiveness = {
            'Electric': { 'Water': 2.0, 'Grass': 0.5, 'Electric': 0.5 },
            'Fire': { 'Grass': 2.0, 'Water': 0.5, 'Fire': 0.5, 'Steel': 2.0 },
            'Water': { 'Fire': 2.0, 'Grass': 0.5, 'Water': 0.5 },
            'Grass': { 'Water': 2.0, 'Fire': 0.5, 'Grass': 0.5 },
            'Psychic': { 'Ghost': 0.5 },
            'Ghost': { 'Psychic': 2.0, 'Normal': 0 },
            'Normal': { 'Ghost': 0 },
            'Steel': { 'Fire': 0.5 }
        };

        if (effectiveness[attackerType] && effectiveness[attackerType][defenderType]) {
            return effectiveness[attackerType][defenderType];
        }
        return 1.0;
    }
}
