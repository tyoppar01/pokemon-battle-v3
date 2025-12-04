import { APIService } from './api.js';
import { MainMenuUI } from './components/index.js';
import { soundManager } from './helpers/soundManager.js';
import { UserHandler, PokemonHandler, BattleHandler } from './handlers/index.js';

/**
 * Main Application Controller
 * Coordinates between handlers and manages application state
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
        
        // Initialize handlers
        this.userHandler = new UserHandler(this);
        this.pokemonHandler = new PokemonHandler(this);
        this.battleHandler = new BattleHandler(this);
        
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
            const { UserUI } = await import('./components/index.js');
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
                if (action === 'selectPlayers') await this.userHandler.showSelectPlayersScreen();
                else if (action === 'createUser') this.userHandler.showCreateUserScreen();
                else if (action === 'viewUsers') await this.userHandler.showViewUsersScreen();
                else if (action === 'viewPokemon') await this.pokemonHandler.showViewPokemonScreen();
            });
        });

        // Select players screen
        document.getElementById('player1Select').addEventListener('change', () => this.userHandler.showPlayerPreview(1));
        document.getElementById('player2Select').addEventListener('change', () => this.userHandler.showPlayerPreview(2));
        document.getElementById('confirmBattleBtn').addEventListener('click', () => this.battleHandler.startBattle());
        document.getElementById('backToMainBtn').addEventListener('click', () => MainMenuUI.show());

        // Create user screen
        document.querySelectorAll('.pokemon-option').forEach(option => {
            option.addEventListener('click', (e) => this.userHandler.togglePokemonSelection(e.target));
        });
        document.getElementById('saveTrainerBtn').addEventListener('click', () => this.userHandler.createTrainer());
        document.getElementById('cancelCreateBtn').addEventListener('click', () => MainMenuUI.show());

        // View users screen
        document.getElementById('backFromUsersBtn').addEventListener('click', () => MainMenuUI.show());

        // View Pokemon screen
        document.getElementById('backFromPokemonBtn').addEventListener('click', () => MainMenuUI.show());

        // Battle screen
        document.querySelectorAll('#battleActionsMenu .menu-option').forEach(option => {
            option.addEventListener('click', (e) => {
                const action = e.target.dataset.action;
                if (action === 'fight') this.battleHandler.showFightMenu();
                else if (action === 'pokemon') this.battleHandler.showPokemonSwitchMenu();
            });
        });

        // Fight menu
        document.querySelectorAll('#fightMenu .menu-option').forEach(option => {
            option.addEventListener('click', (e) => {
                const attack = e.target.dataset.attack;
                const action = e.target.dataset.action;
                if (attack === 'normal') this.battleHandler.executeAttack(false);
                else if (attack === 'special') this.battleHandler.executeAttack(true);
                else if (action === 'back') this.battleHandler.showBattleActions();
            });
        });

        // Pokemon switch menu - delegate event listener for dynamic content
        document.getElementById('pokemonSwitchMenu').addEventListener('click', (e) => {
            const target = e.target.closest('.menu-option');
            if (!target) return;

            const pokemonIndex = target.dataset.pokemonIndex;
            const action = target.dataset.action;

            if (pokemonIndex !== undefined) {
                this.battleHandler.switchPokemon(parseInt(pokemonIndex));
            } else if (action === 'back') {
                this.battleHandler.showBattleActions();
            }
        });
    }

    /**
     * Attach sound effects to all interactive elements
     * Board-level sound management for UI interactions
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
