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

        // Update attack options
        document.getElementById('normalAttackOption').textContent = `1. ${pokemon1.normalAttack?.name || 'Tackle'}`;
        document.getElementById('specialAttackOption').textContent = `2. ${pokemon1.specialAttack?.name || 'No Special Attack'}`;
    }

    /**
     * Update health bar for a player
     * @param {number} playerNum 
     * @param {Pokemon} pokemon 
     */
    static updateHealthBar(playerNum, pokemon) {
        const hpText = document.getElementById(`battlePlayer${playerNum}HP`);
        const hpBar = document.getElementById(`battlePlayer${playerNum}HealthBar`);

        const hpPercent = (pokemon.currentHitPoint / pokemon.maxHitPoint) * 100;
        
        hpText.textContent = `HP: ${pokemon.currentHitPoint}/${pokemon.maxHitPoint}`;
        hpBar.style.width = `${hpPercent}%`;

        // Color based on HP percentage
        if (hpPercent > 50) {
            hpBar.style.backgroundColor = '#00ff00';
        } else if (hpPercent > 25) {
            hpBar.style.backgroundColor = '#ffff00';
        } else {
            hpBar.style.backgroundColor = '#ff0000';
        }
    }

    /**
     * Show battle actions menu
     */
    static showActionsMenu() {
        document.getElementById('battleActionsMenu').classList.remove('hidden');
        document.getElementById('fightMenu').classList.add('hidden');
    }

    /**
     * Show fight menu with attack options
     */
    static showFightMenu() {
        document.getElementById('battleActionsMenu').classList.add('hidden');
        document.getElementById('fightMenu').classList.remove('hidden');
    }

    /**
     * Update battle log with messages
     * @param {Array} battleLog 
     */
    static updateLog(battleLog) {
        const terminal = document.getElementById('battleLogTerminal');
        terminal.innerHTML = battleLog.map(entry => 
            `<div class="log-entry ${entry.type}">${entry.message}</div>`
        ).join('');
        terminal.scrollTop = terminal.scrollHeight;
    }
}
