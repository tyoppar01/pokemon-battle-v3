/**
 * Character Model
 * Represents a Pokemon trainer
 */
export class Character {
    /**
     * 
     * @param {string} name - Trainer name
     * @param {string} gender - Trainer gender
     */
    constructor(name, gender) {
        this.name = name;
        this.gender = gender;
        this.pokePockets = [];
    }

    /**
     * 
     * @param {Pokemon} pokemon - Pokemon to add to team
     */
    addPokemon(pokemon) {
        this.pokePockets.push(pokemon);
    }
}
