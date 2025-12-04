/**
 * UI Component - Battle Screen
 */
import { PokemonSprites } from '../helpers/pokemonSprites.js';

export class BattleUI {

    /**
     * Show battle screen
     */
    static show() {
        document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
        document.getElementById('battleScreen').classList.add('active');
    }

    /**
     * Update battle display with Pokemon info
     * @param {Pokemon} pokemon1 
     * @param {Pokemon} pokemon2 
     */
    static updateDisplay(pokemon1, pokemon2) {
        // Update names
        document.getElementById('battlePlayer1Name').textContent = pokemon1.name;
        document.getElementById('battlePlayer2Name').textContent = pokemon2.name;

        // Update sprites
        document.getElementById('battlePlayer1Sprite').src = PokemonSprites.getSpriteUrl(pokemon1.name);
        document.getElementById('battlePlayer2Sprite').src = PokemonSprites.getSpriteUrl(pokemon2.name);

        // Update HP
        this.updateHealthBar(1, pokemon1);
        this.updateHealthBar(2, pokemon2);
    }

    /**
     * Update fight menu with current player's attacks
     * @param {Pokemon} currentPokemon - The active Pokemon whose turn it is
     */
    static updateFightMenu(currentPokemon) {
        document.getElementById('normalAttackOption').textContent = `1. ${currentPokemon.normalAttack?.name || 'Tackle'}`;
        document.getElementById('specialAttackOption').textContent = `2. ${currentPokemon.specialAttack?.name || currentPokemon.specialSkill || 'Special Attack'}`;
    }

    /**
     * Update health bar for a player
     * @param {number} playerNum 
     * @param {Pokemon} pokemon 
     */
    static updateHealthBar(playerNum, pokemon) {
        const hpText = document.getElementById(`battlePlayer${playerNum}HP`);
        const hpBar = document.getElementById(`battlePlayer${playerNum}HealthBar`);

        // Clamp HP to 0 minimum
        const currentHP = Math.max(0, pokemon.currentHitPoint);
        const hpPercent = (currentHP / pokemon.maxHitPoint) * 100;
        
        hpText.textContent = `HP: ${currentHP}/${pokemon.maxHitPoint}`;
        hpBar.style.width = `${hpPercent}%`;

        // Color based on HP percentage
        hpBar.style.backgroundColor = hpPercent > 50 ? '#00ff00' : hpPercent > 25 ? '#ffff00' : '#ff0000';
    }

    /**
     * Show battle actions menu
     */
    static showActionsMenu() {
        document.getElementById('battleActionsMenu').classList.remove('hidden');
        document.getElementById('fightMenu').classList.add('hidden');
        document.getElementById('pokemonSwitchMenu').classList.add('hidden');
    }

    /**
     * Show fight menu with attack options
     */
    static showFightMenu() {
        document.getElementById('battleActionsMenu').classList.add('hidden');
        document.getElementById('fightMenu').classList.remove('hidden');
        document.getElementById('pokemonSwitchMenu').classList.add('hidden');
    }

    /**
     * Show Pokemon switch menu
     * @param {Array} availablePokemon - List of Pokemon available to switch
     * @param {boolean} isForced - Whether this is a forced switch (Pokemon fainted)
     */
    static showPokemonSwitchMenu(availablePokemon, isForced = false) {
        document.getElementById('battleActionsMenu').classList.add('hidden');
        document.getElementById('fightMenu').classList.add('hidden');
        document.getElementById('pokemonSwitchMenu').classList.remove('hidden');

        const switchList = document.getElementById('pokemonSwitchList');
        const header = document.querySelector('#pokemonSwitchMenu .menu-header');
        
        // Update header based on forced or voluntary switch
        header.textContent = isForced ? 'CHOOSE NEXT POKEMON!' : 'SWITCH POKEMON';
        
        // Populate Pokemon list
        switchList.innerHTML = availablePokemon.map((pokemon, index) => `
            <div class="menu-option" data-pokemon-index="${index}">
                ${index + 1}. ${pokemon.name} (HP: ${pokemon.currentHitPoint}/${pokemon.maxHitPoint})
            </div>
        `).join('');

        // Hide back button if forced switch
        const backButton = document.querySelector('#pokemonSwitchMenu [data-action="back"]');
        backButton.style.display = isForced ? 'none' : 'block';
    }

    /**
     * Update battle log with messages
     * @param {Array} battleLog 
     */
    static updateLog(battleLog) {
        const terminal = document.getElementById('battleLogTerminal');
        terminal.innerHTML = battleLog.map(entry => {
            // Handle both string and object formats
            if (typeof entry === 'string') {
                return `<div class="log-entry">${entry}</div>`;
            } else {
                return `<div class="log-entry ${entry.type || ''}">${entry.message || entry}</div>`;
            }
        }).join('');
        terminal.scrollTop = terminal.scrollHeight;
    }
}
