// battle.js - Battle Logic
// Based on C# PokeBattle implementation with turn-based combat system
import { TypeEffectiveness } from './models.js';

export { PokeBattle };

const BattleStatus = {
    NotStarted: 'NotStarted',
    InProgress: 'InProgress',
    Player1Won: 'Player1Won',
    Player2Won: 'Player2Won'
};

class PokeBattle {
    constructor(player1, player2) {
        // Store both players (Character objects)
        this.player1 = player1;
        this.player2 = player2;
        
        // Battle state
        this.status = BattleStatus.NotStarted;
        this.activePokemon1 = null;  // Player 1's active Pokemon
        this.activePokemon2 = null;  // Player 2's active Pokemon
        this.turnCount = 0;           // Total number of turns
        this.currentTurn = 1;         // Current player's turn (1 or 2)
        this.battleLog = [];          // Log of all battle actions
    }

    // ===================== Battle Initialization ===================== //
    
    /**
     * Start the battle
     * Validates both players have Pokemon and sets up initial state
     */
    start() {
        // Validation: Both players must have at least one Pokemon
        if (!this.player1.pokePockets || this.player1.pokePockets.length === 0 ||
            !this.player2.pokePockets || this.player2.pokePockets.length === 0) {
            throw new Error('Both players must have at least one Pokemon to battle.');
        }

        // Set initial active Pokemon (first in each player's pocket)
        this.activePokemon1 = this.player1.pokePockets[0];
        this.activePokemon2 = this.player2.pokePockets[0];
        
        // Initialize battle state
        this.status = BattleStatus.InProgress;
        this.turnCount = 1;
        this.currentTurn = 1;
        
        // Log battle start
        this.logAction(`Battle started! ${this.player1.name}'s ${this.activePokemon1.name} vs ${this.player2.name}'s ${this.activePokemon2.name}`);
    }

    // ===================== Pokemon Management Methods ===================== //
    
    /**
     * Switch active Pokemon for a player
     * @param {number} playerNumber - Player number (1 or 2)
     * @param {Pokemon} newPokemon - New Pokemon to switch to
     * @param {boolean} changeTurn - Whether to switch turn after switching Pokemon
     */
    switchPokemon(playerNumber, newPokemon, changeTurn = false) {
        // Validation: Battle must be in progress
        if (this.status !== BattleStatus.InProgress) {
            throw new Error('Battle is not in progress.');
        }

        const player = playerNumber === 1 ? this.player1 : this.player2;
        
        // Validation: Pokemon must be in player's pocket
        if (!player.pokePockets.includes(newPokemon)) {
            throw new Error('Pokemon not found in player\'s pocket.');
        }

        // Validation: Cannot switch to a defeated Pokemon
        if (this.isPokemonDefeated(newPokemon)) {
            throw new Error('Cannot switch to a defeated Pokemon.');
        }

        // Swap to new active Pokemon
        if (playerNumber === 1) {
            this.activePokemon1 = newPokemon;
        } else {
            this.activePokemon2 = newPokemon;
        }

        this.logAction(`${player.name} switched to ${newPokemon.name}!`);
        
        // Switch turn if requested (only if not a forced switch due to fainting)
        if (changeTurn) {
            this.switchTurn();
        }
    }

    /**
     * Check if current player needs to switch Pokemon (active Pokemon fainted)
     * @returns {number|null} Player number that needs to switch, or null if none
     */
    needsForcedSwitch() {
        if (this.isPokemonDefeated(this.activePokemon1) && this.hasAlivePokemon(this.player1.pokePockets)) {
            return 1;
        }
        if (this.isPokemonDefeated(this.activePokemon2) && this.hasAlivePokemon(this.player2.pokePockets)) {
            return 2;
        }
        return null;
    }

    /**
     * Get available Pokemon for switching (alive Pokemon that aren't active)
     * @param {number} playerNumber - Player number (1 or 2)
     * @returns {Array} Array of available Pokemon
     */
    getAvailablePokemon(playerNumber) {
        const player = playerNumber === 1 ? this.player1 : this.player2;
        const activePokemon = playerNumber === 1 ? this.activePokemon1 : this.activePokemon2;
        
        return player.pokePockets.filter(p => 
            p !== activePokemon && !this.isPokemonDefeated(p)
        );
    }

    // ===================== Attack Methods ===================== //
    
    /**
     * Perform a normal attack
     * Current turn's Pokemon attacks the opponent's Pokemon
     */
    attack() {
        // Validation: Battle must be in progress
        if (this.status !== BattleStatus.InProgress) {
            return { success: false, message: 'Battle is not in progress' };
        }

        // Determine attacker and defender based on current turn
        const attacker = this.currentTurn === 1 ? this.activePokemon1 : this.activePokemon2;
        const defender = this.currentTurn === 1 ? this.activePokemon2 : this.activePokemon1;
        const attackerOwner = this.currentTurn === 1 ? this.player1.name : this.player2.name;
        const defenderOwner = this.currentTurn === 1 ? this.player2.name : this.player1.name;

        // Calculate damage and apply to defender
        const damage = this.calculateDamage(attacker, defender);
        defender.currentHitPoint -= damage;

        // Log attack action
        this.logAction(`${attackerOwner}'s ${attacker.name} attacked ${defenderOwner}'s ${defender.name} for ${damage} damage!`);

        // Check if defender is defeated
        if (this.isPokemonDefeated(defender)) {
            this.logAction(`${defenderOwner}'s ${defender.name} was defeated!`);
            this.checkBattleEnd();
        } else {
            this.switchTurn();
        }

        return { success: true, damage };
    }

    /**
     * Use special attack with type effectiveness
     * Deals 1.5x damage with type effectiveness multiplier
     */
    useSpecialAttack() {
        // Validation: Battle must be in progress
        if (this.status !== BattleStatus.InProgress) {
            throw new Error('Battle is not in progress.');
        }

        // Determine attacker and defender
        const attacker = this.currentTurn === 1 ? this.activePokemon1 : this.activePokemon2;
        const defender = this.currentTurn === 1 ? this.activePokemon2 : this.activePokemon1;
        const attackerOwner = this.currentTurn === 1 ? this.player1.name : this.player2.name;
        const defenderOwner = this.currentTurn === 1 ? this.player2.name : this.player1.name;

        // Get the special attack type (use Pokemon's type if special attack not specified)
        const attackType = attacker.specialAttack ? attacker.specialAttack.type : attacker.type;

        // Calculate type effectiveness
        const typeMultiplier = TypeEffectiveness.getMultiplier(attackType, defender.type);

        // Calculate damage: base damage * 1.5 (special multiplier) * type effectiveness
        const baseDamage = this.calculateDamage(attacker, defender);
        const specialDamage = Math.floor(baseDamage * 1.5 * typeMultiplier);
        defender.currentHitPoint -= specialDamage;

        // Log special attack
        const skillName = attacker.specialSkill || attacker.specialAttack?.name || 'Special Attack';
        this.logAction(`${attackerOwner}'s ${attacker.name} used ${skillName} for ${specialDamage} damage!`);

        // Log type effectiveness message
        if (typeMultiplier > 1) {
            this.logAction("It's super effective!");
        } else if (typeMultiplier < 1 && typeMultiplier > 0) {
            this.logAction("It's not very effective...");
        } else if (typeMultiplier === 0) {
            this.logAction("It doesn't affect the opponent!");
        }

        // Check if defender is defeated
        if (this.isPokemonDefeated(defender)) {
            this.logAction(`${defenderOwner}'s ${defender.name} was defeated!`);
            this.checkBattleEnd();
        } else {
            this.switchTurn();
        }

        return { success: true, damage: specialDamage, effectiveness: typeMultiplier };
    }

    /**
     * Calculate base damage
     * Formula: (attacker.attack * 100) / (100 + defender.defense)
     * @param {Pokemon} attacker - Attacking Pokemon
     * @param {Pokemon} defender - Defending Pokemon
     * @returns {number} Calculated damage
     */
    calculateDamage(attacker, defender) {
        return Math.floor((attacker.attack * 100) / (100 + defender.defense));
    }

    // ===================== Battle State Methods ===================== //
    
    /**
     * Check if battle has ended
     * Battle ends when a player has no more alive Pokemon
     */
    checkBattleEnd() {
        // Check if Player 1's active Pokemon is defeated
        if (this.isPokemonDefeated(this.activePokemon1)) {
            // Check if Player 1 has other alive Pokemon
            if (this.hasAlivePokemon(this.player1.pokePockets)) {
                this.logAction(`${this.player1.name}'s active Pokemon was defeated but has other Pokemon available!`);
                // Don't end battle, allow switch - status remains InProgress
            } else {
                this.status = BattleStatus.Player2Won;
                this.logAction(`${this.player2.name} wins the battle!`);
            }
        }
        // Check if Player 2's active Pokemon is defeated
        else if (this.isPokemonDefeated(this.activePokemon2)) {
            // Check if Player 2 has other alive Pokemon
            if (this.hasAlivePokemon(this.player2.pokePockets)) {
                this.logAction(`${this.player2.name}'s active Pokemon was defeated but has other Pokemon available!`);
                // Don't end battle, allow switch - status remains InProgress
            } else {
                this.status = BattleStatus.Player1Won;
                this.logAction(`${this.player1.name} wins the battle!`);
            }
        }
    }

    /**
     * Check if a Pokemon is defeated (HP <= 0)
     * @param {Pokemon} pokemon - Pokemon to check
     * @returns {boolean} True if Pokemon is defeated
     */
    isPokemonDefeated(pokemon) {
        return pokemon.currentHitPoint <= 0;
    }

    /**
     * Check if player has any alive Pokemon
     * @param {Array} pokemonList - List of Pokemon to check
     * @returns {boolean} True if at least one Pokemon is alive
     */
    hasAlivePokemon(pokemonList) {
        return pokemonList.some(p => !this.isPokemonDefeated(p));
    }

    /**
     * Get the winner of the battle
     * @returns {Character|null} Winner Character or null if battle not ended
     */
    getWinner() {
        if (this.status === BattleStatus.Player1Won) return this.player1;
        if (this.status === BattleStatus.Player2Won) return this.player2;
        return null;
    }

    /**
     * Switch to the next player's turn
     * Increments turn count and toggles current turn
     */
    switchTurn() {
        this.currentTurn = this.currentTurn === 1 ? 2 : 1;
        this.turnCount++;
    }

    // ===================== Logging Methods ===================== //
    
    /**
     * Log a battle action
     * @param {string} action - Action description
     */
    logAction(action) {
        this.battleLog.push(`Turn ${this.turnCount}: ${action}`);
    }

    /**
     * Get battle log
     * @returns {Array} Copy of battle log
     */
    getBattleLog() {
        return [...this.battleLog];
    }

    /**
     * Get current battle status
     * @returns {string} Battle status
     */
    getStatus() {
        return this.status;
    }
}
