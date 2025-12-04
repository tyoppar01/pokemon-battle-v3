/**
 * Pokemon Model
 * Core Pokemon entity with stats and battle properties
 */
import { Attack } from './Attack.js';

export class Pokemon {
    /**
     * 
     * @param {string} name - Pokemon name
     * @param {string} type - Pokemon type (Electric, Fire, etc.)
     * @param {number} level - Pokemon level
     * @param {number} hp - Maximum hit points
     * @param {number} attack - Attack stat
     * @param {number} defense - Defense stat
     * @param {number} speed - Speed stat
     */
    constructor(name, type, level, hp, attack, defense, speed) {
        this.name = name;
        this.type = type;
        this.level = level;
        this.maxHitPoint = hp;
        this.currentHitPoint = hp;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.normalAttack = new Attack("Tackle", 40);
        this.specialAttack = null;
    }

    /**
     * 
     * @param {number} damage - Damage to apply
     */
    takeDamage(damage) {
        this.currentHitPoint = Math.max(0, this.currentHitPoint - damage);
    }

    /**
     * 
     * @returns {boolean} Whether Pokemon is still alive
     */
    isAlive() {
        return this.currentHitPoint > 0;
    }
}
