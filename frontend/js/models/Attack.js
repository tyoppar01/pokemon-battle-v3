/**
 * Attack Model
 * Represents a Pokemon attack/move
 */
export class Attack {
    /**
     * 
     * @param {string} name - Attack name
     * @param {number} power - Attack power/damage
     */
    constructor(name, power) {
        this.name = name;
        this.power = power;
    }
}
