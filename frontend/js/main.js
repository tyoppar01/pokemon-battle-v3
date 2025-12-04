import { Character, Pokemon, Attack } from './models.js';
import { PokeBattle } from './battle.js';
import { APIService } from './api.js';
import { MainMenuUI, BattleUI, UserUI, PokemonUI } from './components/index.js';
import { soundManager } from './helpers/soundManager.js';

/**
 * Main Application Controller
 */
class BattleApp {

    constructor() {
        this.apiService = new APIService();
        this.users = [];
        this.selectedTeam = [];
        this.battle = null;
        this.soundManager = soundManager;
        this.currentAvailablePokemon = null;
        this.currentSwitchingPlayer = null;
        this.isForcedSwitch = false;
        this.init();
    }

    async init() {
        MainMenuUI.showLogo();
        await this.loadUsers();
        this.attachEventListeners();
        this.attachSoundEffects();
        MainMenuUI.show();
    }

    /**
     * Load all users from the API
     */
    async loadUsers() {
        try {
            this.users = await this.apiService.getAllUsers();
            UserUI.populateUserSelects(this.users);
        } catch (error) {
            console.error('Failed to load users:', error);
        }
    }

    /**
     * Attach event listeners to various UI elements
     */
    attachEventListeners() {
        // Main menu
        document.querySelectorAll('#mainMenu .menu-option').forEach(option => {
            option.addEventListener('click', async (e) => {
                const action = e.target.dataset.action;
                console.log('Menu option clicked:', action);
                if (action === 'selectPlayers') await this.showSelectPlayersScreen();
                else if (action === 'createUser') this.showCreateUserScreen();
                else if (action === 'viewUsers') await this.showViewUsersScreen();
                else if (action === 'viewPokemon') await this.showViewPokemonScreen();
            });
        });

        // Select players screen
        document.getElementById('player1Select').addEventListener('change', () => this.showPlayerPreview(1));
        document.getElementById('player2Select').addEventListener('change', () => this.showPlayerPreview(2));
        document.getElementById('confirmBattleBtn').addEventListener('click', () => this.startBattle());
        document.getElementById('backToMainBtn').addEventListener('click', () => MainMenuUI.show());

        // Create user screen
        document.querySelectorAll('.pokemon-option').forEach(option => {
            option.addEventListener('click', (e) => this.togglePokemonSelection(e.target));
        });
        document.getElementById('saveTrainerBtn').addEventListener('click', () => this.createTrainer());
        document.getElementById('cancelCreateBtn').addEventListener('click', () => MainMenuUI.show());

        // View users screen
        document.getElementById('backFromUsersBtn').addEventListener('click', () => MainMenuUI.show());

        // View Pokemon screen
        document.getElementById('backFromPokemonBtn').addEventListener('click', () => MainMenuUI.show());

        // Battle screen
        document.querySelectorAll('#battleActionsMenu .menu-option').forEach(option => {
            option.addEventListener('click', (e) => {
                const action = e.target.dataset.action;
                if (action === 'fight') this.showFightMenu();
                else if (action === 'pokemon') this.showPokemonSwitchMenu();
            });
        });

        // Fight menu
        document.querySelectorAll('#fightMenu .menu-option').forEach(option => {
            option.addEventListener('click', (e) => {
                const attack = e.target.dataset.attack;
                const action = e.target.dataset.action;
                if (attack === 'normal') this.executeAttack(false);
                else if (attack === 'special') this.executeAttack(true);
                else if (action === 'back') this.showBattleActions();
            });
        });

        // Pokemon switch menu - delegate event listener for dynamic content
        document.getElementById('pokemonSwitchMenu').addEventListener('click', (e) => {
            const target = e.target.closest('.menu-option');
            if (!target) return;

            const pokemonIndex = target.dataset.pokemonIndex;
            const action = target.dataset.action;

            if (pokemonIndex !== undefined) {
                this.switchPokemon(parseInt(pokemonIndex));
            } else if (action === 'back') {
                this.showBattleActions();
            }
        });
    }

    /**
     * Show the select players screen
     */
    async showSelectPlayersScreen() {
        await this.loadUsers();
        UserUI.showSelectPlayers();
    }

    /**
     * Show the create user screen
     */
    showCreateUserScreen() {
        this.selectedTeam = [];
        UserUI.showCreateUser();
    }

    /**
     * Show view users screen
     */
    async showViewUsersScreen() {
        console.log('showViewUsersScreen called');
        await this.loadUsers();
        console.log('Users loaded:', this.users);
        UserUI.showViewUsers(this.users);
        console.log('UserUI.showViewUsers called');
        
        // Attach delete button listeners
        this.attachDeleteUserListeners();
    }

    /**
     * Attach event listeners to delete user buttons
     */
    attachDeleteUserListeners() {
        document.querySelectorAll('.delete-user-btn').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                e.stopPropagation();
                const userId = e.target.dataset.userId;
                await this.deleteUser(userId);
            });
        });
    }

    /**
     * Delete a user
     * @param {string} userId 
     */
    async deleteUser(userId) {
        const user = this.users.find(u => u.id === userId);
        if (!user) return;
        
        const confirmed = confirm(`Are you sure you want to delete trainer "${user.name}"? This cannot be undone.`);
        if (!confirmed) return;
        
        try {
            await this.apiService.deleteUser(userId);
            this.soundManager.playConfirm();
            alert(`✓ Trainer "${user.name}" has been deleted!`);
            // Refresh the users list
            await this.showViewUsersScreen();
        } catch (error) {
            console.error('Failed to delete user:', error);
            this.soundManager.playError();
            alert('✗ Failed to delete trainer. Please try again.');
        }
    }

    /**
     * Show view Pokemon screen
     */
    async showViewPokemonScreen() {
        console.log('showViewPokemonScreen called');
        try {
            const allPokemon = await this.apiService.getAllPlayablePokemon();
            console.log('Pokemon loaded:', allPokemon);
            await PokemonUI.show(allPokemon);
            console.log('PokemonUI.show called');
        } catch (error) {
            console.error('Failed to load Pokemon:', error);
            await PokemonUI.show([]);
        }
    }

    /**
     * Show player preview when a trainer is selected
     * @param {number} playerNum 
     */
    async showPlayerPreview(playerNum) {
        const select = document.getElementById(`player${playerNum}Select`);
        const userId = select.value;

        if (!userId) {
            UserUI.showPlayerPreview(playerNum, null);
            return;
        }

        try {
            // Find user from the loaded users list
            const user = this.users.find(u => u.id === userId);
            if (user) {
                UserUI.showPlayerPreview(playerNum, user);
            } else {
                // If user not found, clear preview
                UserUI.showPlayerPreview(playerNum, null);
            }
        } catch (error) {
            console.error('Error loading Pokemon:', error);
            UserUI.showPlayerPreview(playerNum, null);
        }
    }

    /**
     * Toggle Pokemon selection for the new trainer
     * @param {HTMLElement} element 
     */
    togglePokemonSelection(element) {
        const pokemonName = element.dataset.pokemon;
        
        if (element.classList.contains('selected')) {
            // Remove from team
            element.classList.remove('selected');
            this.selectedTeam = this.selectedTeam.filter(p => p !== pokemonName);
        } else {
            // Add to team (if not full)
            if (this.selectedTeam.length >= 6) {
                alert('Maximum 6 Pokemon allowed!');
                return;
            }
            element.classList.add('selected');
            this.selectedTeam.push(pokemonName);
        }

        UserUI.updateTeamDisplay(this.selectedTeam);
    }

    /**
     * Create a new trainer with selected Pokemon
     */
    async createTrainer() {
        const name = document.getElementById('newTrainerName').value.trim();
        const gender = document.getElementById('newTrainerGender').value;

        if (!name) {
            this.soundManager.playError();
            alert('Please enter a trainer name!');
            return;
        }

        if (this.selectedTeam.length === 0) {
            this.soundManager.playError();
            alert('Please select at least one Pokemon!');
            return;
        }

        try {
            // Create user
            const user = await this.apiService.createUser(name, gender);
            
            // Add Pokemon to user
            for (const pokemonType of this.selectedTeam) {
                await this.apiService.addPokemonToUser(user.id, pokemonType);
            }

            this.soundManager.playConfirm();
            alert(`Trainer ${name} created successfully with ${this.selectedTeam.length} Pokemon!`);
            await this.loadUsers();
            MainMenuUI.show();
        } catch (error) {
            this.soundManager.playError();
            alert('Failed to create trainer. Please try again.');
            console.error(error);
        }
    }    
    
    /**
     * Start a battle between two selected trainers
     */
    async startBattle() {
        const player1Id = document.getElementById('player1Select').value;
        const player2Id = document.getElementById('player2Select').value;

        if (!player1Id || !player2Id) {
            this.soundManager.playError();
            alert('Please select both trainers!');
            return;
        }

        if (player1Id === player2Id) {
            this.soundManager.playError();
            alert('Please select different trainers!');
            return;
        }

        // Play battle start sound
        this.soundManager.playBattleStart();

        try {
            // Get user data
            const user1 = this.users.find(u => u.id === player1Id);
            const user2 = this.users.find(u => u.id === player2Id);
            
            const pokemon1List = await this.apiService.getUserPokemon(player1Id);
            const pokemon2List = await this.apiService.getUserPokemon(player2Id);

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
            this.battle = new PokeBattle(player1, player2);
            this.battle.start();

            // Setup battle UI
            BattleUI.show();
            BattleUI.updateDisplay(this.battle.activePokemon1, this.battle.activePokemon2);
            BattleUI.showActionsMenu();
            BattleUI.updateLog(this.battle.getBattleLog());

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
        BattleUI.showFightMenu();
    }

    /**
     * Show Pokemon switch menu
     */
    showPokemonSwitchMenu() {
        if (!this.battle) return;
        
        const availablePokemon = this.battle.getAvailablePokemon(this.battle.currentTurn);
        
        if (availablePokemon.length === 0) {
            this.soundManager.playError();
            alert('No other Pokemon available to switch!');
            return;
        }
        
        this.currentAvailablePokemon = availablePokemon;
        BattleUI.showPokemonSwitchMenu(availablePokemon, false);
    }

    /**
     * Show forced Pokemon switch menu (when active Pokemon faints)
     * @param {number} playerNumber - Player who needs to switch
     */
    showForcedPokemonSwitch(playerNumber) {
        if (!this.battle) return;
        
        const availablePokemon = this.battle.getAvailablePokemon(playerNumber);
        const playerName = playerNumber === 1 ? this.battle.player1.name : this.battle.player2.name;
        
        this.soundManager.playError();
        alert(`${playerName}'s Pokemon fainted! Choose your next Pokemon!`);
        
        this.currentAvailablePokemon = availablePokemon;
        this.currentSwitchingPlayer = playerNumber;
        this.isForcedSwitch = true;
        
        BattleUI.showPokemonSwitchMenu(availablePokemon, true);
    }

    /**
     * Switch to a different Pokemon
     * @param {number} pokemonIndex - Index in the available Pokemon list
     */
    switchPokemon(pokemonIndex) {
        if (!this.battle || !this.currentAvailablePokemon) return;
        
        const selectedPokemon = this.currentAvailablePokemon[pokemonIndex];
        if (!selectedPokemon) return;
        
        this.soundManager.playSelect();
        
        // Determine which player is switching
        const playerNumber = this.isForcedSwitch ? this.currentSwitchingPlayer : this.battle.currentTurn;
        
        // Perform the switch - don't change turn if it's a forced switch
        const shouldChangeTurn = !this.isForcedSwitch;
        this.battle.switchPokemon(playerNumber, selectedPokemon, shouldChangeTurn);
        
        // Update display
        BattleUI.updateDisplay(this.battle.activePokemon1, this.battle.activePokemon2);
        BattleUI.updateLog(this.battle.getBattleLog());
        
        // Reset switch state
        this.currentAvailablePokemon = null;
        this.currentSwitchingPlayer = null;
        this.isForcedSwitch = false;
        
        // Check if battle ended or if another forced switch is needed
        if (this.battle.getStatus() !== 'InProgress') {
            setTimeout(() => {
                const winner = this.battle.getStatus() === 'Player1Won' ? this.battle.player1.name : this.battle.player2.name;
                alert(`🏆 ${winner} wins the battle!`);
                MainMenuUI.show();
            }, 1000);
        } else {
            // Check if a forced switch is needed
            const needsSwitch = this.battle.needsForcedSwitch();
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
        if (!this.battle) return;

        // Play attack sound
        this.soundManager.playAttack();

        const result = isSpecial 
            ? this.battle.useSpecialAttack()
            : this.battle.attack();
        
        if (result.success) {
            // Play damage sound after a short delay
            setTimeout(() => this.soundManager.playDamage(), 150);
            
            BattleUI.updateDisplay(this.battle.activePokemon1, this.battle.activePokemon2);
            BattleUI.updateLog(this.battle.getBattleLog());
            
            // Check if battle ended
            if (this.battle.getStatus() !== 'InProgress') {
                setTimeout(() => {
                    const winner = this.battle.getStatus() === 'Player1Won' ? this.battle.player1.name : this.battle.player2.name;
                    alert(`🏆 ${winner} wins the battle!`);
                    MainMenuUI.show();
                }, 1000);
            } else {
                // Check if a forced switch is needed (Pokemon fainted)
                const needsSwitch = this.battle.needsForcedSwitch();
                if (needsSwitch) {
                    setTimeout(() => this.showForcedPokemonSwitch(needsSwitch), 500);
                } else {
                    BattleUI.showActionsMenu();
                }
            }
        }
    }    /**
     * Attach sound effects to all interactive elements
     */
    attachSoundEffects() {
        // Add click sounds to all menu options
        document.querySelectorAll('.menu-option:not(.disabled)').forEach(option => {
            option.addEventListener('click', () => this.soundManager.playButtonClick());
            option.addEventListener('mouseenter', () => this.soundManager.playHover());
        });

        // Add sounds to all buttons
        document.querySelectorAll('button.terminal-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                if (btn.classList.contains('secondary')) {
                    this.soundManager.playCancel();
                } else {
                    this.soundManager.playConfirm();
                }
            });
        });

        // Add sounds to delete buttons
        document.addEventListener('click', (e) => {
            if (e.target && e.target.classList && e.target.classList.contains('delete-user-btn')) {
                this.soundManager.playCancel();
            }
        });

        // Add sounds to select dropdowns
        document.querySelectorAll('select.terminal-select').forEach(select => {
            select.addEventListener('change', () => this.soundManager.playSelect());
        });

        // Add sounds to Pokemon options
        document.querySelectorAll('.pokemon-option').forEach(option => {
            option.addEventListener('click', () => this.soundManager.playButtonClick());
        });

        // Add hover sounds to user cards
        document.addEventListener('mouseenter', (e) => {
            if (e.target && e.target.classList && e.target.classList.contains('user-card')) {
                this.soundManager.playHover();
            }
        }, true);
    }
}

// Initialize app when DOM is ready
document.addEventListener('DOMContentLoaded', () => new BattleApp());
