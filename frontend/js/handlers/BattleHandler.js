/**
 * Battle Handler - Manages battle operations and Pokemon switching
 */
import { Character, Pokemon, Attack } from '../models.js';
import { PokeBattle } from '../battle.js';
import { BattleUI, MainMenuUI } from '../components/index.js';

export class BattleHandler {
    constructor(app) {
        this.app = app;
    }

    /**
     * Start a battle between two selected trainers
     */
    async startBattle() {
        const player1Id = document.getElementById('player1Select').value;
        const player2Id = document.getElementById('player2Select').value;

        if (!player1Id || !player2Id) {
            this.app.soundManager.playError();
            alert('Please select both trainers!');
            return;
        }

        if (player1Id === player2Id) {
            this.app.soundManager.playError();
            alert('Please select different trainers!');
            return;
        }

        // Play battle start sound
        this.app.soundManager.playBattleStart();

        try {
            // Get user data
            const user1 = this.app.users.find(u => u.id === player1Id);
            const user2 = this.app.users.find(u => u.id === player2Id);
            
            const pokemon1List = await this.app.apiService.getUserPokemon(player1Id);
            const pokemon2List = await this.app.apiService.getUserPokemon(player2Id);

            if (pokemon1List.length === 0 || pokemon2List.length === 0) {
                alert('Both trainers must have at least one Pokemon!');
                return;
            }

            // Transform API Pokemon data to frontend Pokemon model
            const createPokemonFromAPI = (apiPokemon) => {
                const pokemon = new Pokemon(
                    apiPokemon.name,
                    apiPokemon.type,
                    apiPokemon.level,
                    apiPokemon.maxHitPoint,
                    apiPokemon.attack,
                    apiPokemon.defense,
                    apiPokemon.speed
                );
                pokemon.currentHitPoint = apiPokemon.currentHitPoint;
                // Set special attack if available
                if (apiPokemon.specialSkill && apiPokemon.specialSkill !== 'Unknown') {
                    pokemon.specialAttack = new Attack(apiPokemon.specialSkill, 60);
                }
                return pokemon;
            };

            // Create characters with Pokemon data from backend
            const player1 = new Character(user1.name, user1.gender);
            // Add ALL Pokemon from player 1's team
            pokemon1List.forEach(apiPokemon => {
                player1.addPokemon(createPokemonFromAPI(apiPokemon));
            });

            const player2 = new Character(user2.name, user2.gender);
            // Add ALL Pokemon from player 2's team
            pokemon2List.forEach(apiPokemon => {
                player2.addPokemon(createPokemonFromAPI(apiPokemon));
            });

            // Initialize battle
            this.app.battle = new PokeBattle(player1, player2);
            this.app.battle.start();

            // Setup battle UI
            BattleUI.show();
            BattleUI.updateDisplay(this.app.battle.activePokemon1, this.app.battle.activePokemon2);
            BattleUI.showActionsMenu();
            BattleUI.updateLog(this.app.battle.getBattleLog());

        } catch (error) {
            alert('Failed to start battle. Please try again.');
            console.error(error);
        }
    }

    /**     
     * Show battle action menu
     */
    showBattleActions() {
        BattleUI.showActionsMenu();
    }

    /**     
     * Show fight menu
     */
    showFightMenu() {
        if (!this.app.battle) return;
        
        // Get the current player's active Pokemon
        const currentPokemon = this.app.battle.currentTurn === 1 
            ? this.app.battle.activePokemon1 
            : this.app.battle.activePokemon2;
        
        // Update fight menu with current Pokemon's attacks
        BattleUI.updateFightMenu(currentPokemon);
        BattleUI.showFightMenu();
    }

    /**
     * Show Pokemon switch menu
     */
    showPokemonSwitchMenu() {
        if (!this.app.battle) return;
        
        const availablePokemon = this.app.battle.getAvailablePokemon(this.app.battle.currentTurn);
        
        if (availablePokemon.length === 0) {
            this.app.soundManager.playError();
            alert('No other Pokemon available to switch!');
            return;
        }
        
        this.app.currentAvailablePokemon = availablePokemon;
        BattleUI.showPokemonSwitchMenu(availablePokemon, false);
    }

    /**
     * Show forced Pokemon switch menu (when active Pokemon faints)
     * @param {number} playerNumber - Player who needs to switch
     */
    showForcedPokemonSwitch(playerNumber) {
        if (!this.app.battle) return;
        
        const availablePokemon = this.app.battle.getAvailablePokemon(playerNumber);
        const playerName = playerNumber === 1 ? this.app.battle.player1.name : this.app.battle.player2.name;
        
        this.app.soundManager.playError();
        
        this.app.currentAvailablePokemon = availablePokemon;
        this.app.currentSwitchingPlayer = playerNumber;
        this.app.isForcedSwitch = true;
        
        BattleUI.showPokemonSwitchMenu(availablePokemon, true);
    }

    /**
     * Switch to a different Pokemon
     * @param {number} pokemonIndex - Index in the available Pokemon list
     */
    switchPokemon(pokemonIndex) {
        if (!this.app.battle || !this.app.currentAvailablePokemon) return;
        
        const selectedPokemon = this.app.currentAvailablePokemon[pokemonIndex];
        if (!selectedPokemon) return;
        
        this.app.soundManager.playSelect();
        
        // Determine which player is switching
        const playerNumber = this.app.isForcedSwitch ? this.app.currentSwitchingPlayer : this.app.battle.currentTurn;
        
        // Perform the switch - don't change turn if it's a forced switch
        const shouldChangeTurn = !this.app.isForcedSwitch;
        this.app.battle.switchPokemon(playerNumber, selectedPokemon, shouldChangeTurn);
        
        // If this was a forced switch, set the turn to the player who just switched
        // This ensures the player who switched gets to attack, not the opponent
        if (this.app.isForcedSwitch) {
            this.app.battle.setTurnAfterForcedSwitch(playerNumber);
        }
        
        // Update display
        BattleUI.updateDisplay(this.app.battle.activePokemon1, this.app.battle.activePokemon2);
        BattleUI.updateLog(this.app.battle.getBattleLog());
        
        // Reset switch state
        this.app.currentAvailablePokemon = null;
        this.app.currentSwitchingPlayer = null;
        this.app.isForcedSwitch = false;
        
        // Check if battle ended or if another forced switch is needed
        if (this.app.battle.getStatus() !== 'InProgress') {
            setTimeout(() => {
                const winner = this.app.battle.getStatus() === 'Player1Won' ? this.app.battle.player1.name : this.app.battle.player2.name;
                alert(`🏆 ${winner} wins the battle!`);
                MainMenuUI.show();
            }, 1000);
        } else {
            // Check if a forced switch is needed
            const needsSwitch = this.app.battle.needsForcedSwitch();
            if (needsSwitch) {
                setTimeout(() => this.showForcedPokemonSwitch(needsSwitch), 500);
            } else {
                BattleUI.showActionsMenu();
            }
        }
    }

    /**
     * Execute an attack
     * @param {boolean} isSpecial 
     */
    executeAttack(isSpecial) {
        if (!this.app.battle) return;

        // Play attack sound
        this.app.soundManager.playAttack();

        const result = isSpecial 
            ? this.app.battle.useSpecialAttack()
            : this.app.battle.attack();
        
        if (result.success) {
            // Play damage sound after a short delay
            setTimeout(() => this.app.soundManager.playDamage(), 150);
            
            BattleUI.updateDisplay(this.app.battle.activePokemon1, this.app.battle.activePokemon2);
            BattleUI.updateLog(this.app.battle.getBattleLog());
            
            // Check if battle ended
            if (this.app.battle.getStatus() !== 'InProgress') {
                setTimeout(() => {
                    const winner = this.app.battle.getStatus() === 'Player1Won' ? this.app.battle.player1.name : this.app.battle.player2.name;
                    alert(`🏆 ${winner} wins the battle!`);
                    MainMenuUI.show();
                }, 1000);
            } else {
                // Check if a forced switch is needed (Pokemon fainted)
                const needsSwitch = this.app.battle.needsForcedSwitch();
                if (needsSwitch) {
                    setTimeout(() => this.showForcedPokemonSwitch(needsSwitch), 500);
                } else {
                    BattleUI.showActionsMenu();
                }
            }
        }
    }
}
