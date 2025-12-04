import { Character } from './models.js';
import { PokeBattle } from './battle.js';
import { APIService } from './api.js';
import { PokemonSprites } from './pokemonSprites.js';

/**
 * Main Application
 */
class BattleApp {

    constructor() {
        this.apiService = new APIService();
        this.users = [];
        this.selectedTeam = [];
        this.battle = null;
        this.init();
    }

    async init() {
        this.showLogo();
        await this.loadUsers();
        this.attachEventListeners();
        this.showScreen('mainMenu');
    }

    showLogo() {
        const logo = `
╔════════════════════════════════════════════════════════════════════╗
║                                                                    ║
║   ██████╗  ██████╗ ██╗  ██╗███████╗███╗   ███╗ ██████╗ ███╗   ██╗  ║
║   ██╔══██╗██╔═══██╗██║ ██╔╝██╔════╝████╗ ████║██╔═══██╗████╗  ██║  ║
║   ██████╔╝██║   ██║█████╔╝ █████╗  ██╔████╔██║██║   ██║██╔██╗ ██║  ║
║   ██╔═══╝ ██║   ██║██╔═██╗ ██╔══╝  ██║╚██╔╝██║██║   ██║██║╚██╗██║  ║
║   ██║     ╚██████╔╝██║  ██╗███████╗██║ ╚═╝ ██║╚██████╔╝██║ ╚████║  ║
║   ╚═╝      ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝ ╚═════╝ ╚═╝  ╚═══╝  ║
║                                                                    ║
║   ██████╗  █████╗ ████████╗████████╗██╗     ███████╗               ║
║   ██╔══██╗██╔══██╗╚══██╔══╝╚══██╔══╝██║     ██╔════╝               ║
║   ██████╔╝███████║   ██║      ██║   ██║     █████╗                 ║
║   ██╔══██╗██╔══██║   ██║      ██║   ██║     ██╔══╝                 ║
║   ██████╔╝██║  ██║   ██║      ██║   ███████╗███████╗               ║
║   ╚═════╝ ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚══════╝╚══════╝               ║
║                                                                    ║
╚════════════════════════════════════════════════════════════════════╝`;
        document.getElementById('logoArt').textContent = logo;
    }

    /**
     * Load all users from the API
     */
    async loadUsers() {
        try {
            this.users = await this.apiService.getAllUsers();
            this.populateUserSelects();
        } catch (error) {
            console.error('Failed to load users:', error);
        }
    }

    /**
     * Populate the user selection dropdowns
     */
    populateUserSelects() {
        const player1Select = document.getElementById('player1Select');
        const player2Select = document.getElementById('player2Select');

        [player1Select, player2Select].forEach(select => {
            select.innerHTML = '<option value="">Select trainer...</option>';
            this.users.forEach(user => {
                const option = document.createElement('option');
                option.value = user.id;
                option.textContent = `${user.name} (${user.pokemon.length} Pokemon)`;
                select.appendChild(option);
            });
        });
    }

    /**
     * Attach event listeners to various UI elements
     */
    attachEventListeners() {
        // Main menu
        document.querySelectorAll('#mainMenu .menu-option').forEach(option => {
            option.addEventListener('click', (e) => {
                const action = e.target.dataset.action;
                if (action === 'selectPlayers') this.showSelectPlayersScreen();
                else if (action === 'createUser') this.showCreateUserScreen();
                else if (action === 'viewUsers') this.showViewUsersScreen();
                else if (action === 'viewPokemon') this.showViewPokemonScreen();
            });
        });

        // Select players screen
        document.getElementById('player1Select').addEventListener('change', () => this.showPlayerPreview(1));
        document.getElementById('player2Select').addEventListener('change', () => this.showPlayerPreview(2));
        document.getElementById('confirmBattleBtn').addEventListener('click', () => this.startBattle());
        document.getElementById('backToMainBtn').addEventListener('click', () => this.showScreen('mainMenu'));

        // Create user screen
        document.querySelectorAll('.pokemon-option').forEach(option => {
            option.addEventListener('click', (e) => this.togglePokemonSelection(e.target));
        });
        document.getElementById('saveTrainerBtn').addEventListener('click', () => this.createTrainer());
        document.getElementById('cancelCreateBtn').addEventListener('click', () => this.showScreen('mainMenu'));

        // View users screen
        document.getElementById('backFromUsersBtn').addEventListener('click', () => this.showScreen('mainMenu'));

        // View Pokemon screen
        document.getElementById('backFromPokemonBtn').addEventListener('click', () => this.showScreen('mainMenu'));

        // Battle screen
        document.querySelectorAll('#battleActionsMenu .menu-option').forEach(option => {
            option.addEventListener('click', (e) => {
                const action = e.target.dataset.action;
                if (action === 'fight') this.showFightMenu();
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
    }

    // Show a specific screen by ID
    showScreen(screenId) {
        document.querySelectorAll('.screen').forEach(screen => screen.classList.remove('active'));
        document.getElementById(screenId).classList.add('active');
    }

    /**
     * Show the select players screen
     */
    async showSelectPlayersScreen() {
        await this.loadUsers();
        this.showScreen('selectPlayersScreen');
    }

    /**
     * Show player preview when a trainer is selected
     * @param {number} playerNum 
     */
    async showPlayerPreview(playerNum) {
        const select = document.getElementById(`player${playerNum}Select`);
        const preview = document.getElementById(`player${playerNum}Preview`);
        const userId = select.value;

        if (!userId) {
            preview.innerHTML = '<p style="color: #666;">Select a trainer...</p>';
            return;
        }

        try {
            const pokemon = await this.apiService.getUserPokemon(userId);
            const user = this.users.find(u => u.id === userId);
            
            preview.innerHTML = `
                <div class="trainer-name">${user.name}</div>
                <div class="pokemon-count">${pokemon.length} Pokemon in team</div>
                ${pokemon.map((p, i) => `<div class="pokemon-item">  ${i + 1}. ${p.name} (${p.type})</div>`).join('')}
            `;
        } catch (error) {
            preview.innerHTML = '<p style="color: #ff0000;">Error loading Pokemon</p>';
        }
    }

    /**
     * Show the create user screen
     */
    showCreateUserScreen() {
        this.selectedTeam = [];
        document.getElementById('newTrainerName').value = '';
        document.getElementById('teamDisplay').innerHTML = '<p style="color: #666;">No Pokemon selected yet</p>';
        document.getElementById('teamCounter').textContent = '0';
        document.querySelectorAll('.pokemon-option').forEach(opt => opt.classList.remove('selected'));
        this.showScreen('createUserScreen');
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

        this.updateTeamDisplay();
    }

    /**
     * Update the selected team display
     */
    updateTeamDisplay() {
        const teamDisplay = document.getElementById('teamDisplay');
        const teamCounter = document.getElementById('teamCounter');

        teamCounter.textContent = this.selectedTeam.length;

        if (this.selectedTeam.length === 0) {
            teamDisplay.innerHTML = '<p style="color: #666;">No Pokemon selected yet</p>';
        } else {
            teamDisplay.innerHTML = this.selectedTeam.map((pokemon, i) => `
                <div class="team-item">
                    <span>${i + 1}. ${pokemon}</span>
                </div>
            `).join('');
        }
    }

    /**     
     * Create a new trainer with selected Pokemon
     */
    async createTrainer() {
        const name = document.getElementById('newTrainerName').value.trim();
        const gender = document.getElementById('newTrainerGender').value;

        if (!name) {
            alert('Please enter a trainer name!');
            return;
        }

        if (this.selectedTeam.length === 0) {
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

            alert(`Trainer ${name} created successfully with ${this.selectedTeam.length} Pokemon!`);
            await this.loadUsers();
            this.showScreen('mainMenu');
        } catch (error) {
            alert('Failed to create trainer. Please try again.');
            console.error(error);
        }
    }

    /**     
     * Show all trainers and their Pokemon
     */
    async showViewUsersScreen() {
        const usersList = document.getElementById('usersList');
        
        if (this.users.length === 0) {
            usersList.innerHTML = '<p style="color: #666; text-align: center; padding: 40px;">No trainers found. Create one first!</p>';
        } else {
            usersList.innerHTML = this.users.map(user => `
                <div class="user-card">
                    <div class="user-header">${user.name}</div>
                    <div class="user-gender">Gender: ${user.gender}</div>
                    <div class="pokemon-list">
                        <div style="color: #ffff00; margin-bottom: 10px;">Pokemon Team (${user.pokemon.length}):</div>
                        ${user.pokemon.map((p, i) => `
                            <div class="pokemon-item">  ✓ ${p.name} (${p.type}) - Level ${p.level}</div>
                        `).join('')}
                    </div>
                </div>
            `).join('');
        }

        this.showScreen('viewUsersScreen');
    }

    /**     
     * Show all available Pokemon
     */
    async showViewPokemonScreen() {
        const pokemonList = document.getElementById('pokemonList');
        
        try {
            const allPokemon = await this.apiService.getAllPlayablePokemon();
            
            if (allPokemon.length === 0) {
                pokemonList.innerHTML = '<p style="color: #666; text-align: center; padding: 40px;">No Pokemon available.</p>';
            } else {
                pokemonList.innerHTML = allPokemon.map((pokemon, index) => `
                    <div class="pokemon-card">
                        <div class="pokemon-header">${index + 1}. ${pokemon.name}</div>
                        <div class="pokemon-type">Type: ${pokemon.type}</div>
                        <div class="pokemon-stats">
                            <div class="stat-row">HP: ${pokemon.baseMaxHP}</div>
                            <div class="stat-row">Attack: ${pokemon.baseAttack}</div>
                            <div class="stat-row">Defense: ${pokemon.baseDefense}</div>
                            <div class="stat-row">Speed: ${pokemon.baseSpeed}</div>
                        </div>
                        <div class="pokemon-attacks">
                            <div class="attack-label">Attacks:</div>
                            <div class="attack-item">• ${pokemon.normalAttack}</div>
                            <div class="attack-item special">• ${pokemon.specialAttack}</div>
                        </div>
                    </div>
                `).join('');
            }
        } catch (error) {
            console.error('Failed to load Pokemon:', error);
            pokemonList.innerHTML = '<p style="color: #ff0000; text-align: center; padding: 40px;">Failed to load Pokemon data.</p>';
        }

        this.showScreen('viewPokemonScreen');
    }

    /**     
     * Start a battle between two selected trainers
     */
    async startBattle() {
        const player1Id = document.getElementById('player1Select').value;
        const player2Id = document.getElementById('player2Select').value;

        if (!player1Id || !player2Id) {
            alert('Please select both trainers!');
            return;
        }

        if (player1Id === player2Id) {
            alert('Please select different trainers!');
            return;
        }

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

            // Create characters with Pokemon data from backend
            const player1 = new Character(user1.name, user1.gender);
            player1.addPokemon(pokemon1List[0]);

            const player2 = new Character(user2.name, user2.gender);
            player2.addPokemon(pokemon2List[0]);

            // Initialize battle
            this.battle = new PokeBattle(player1, player2);
            this.battle.start();

            // Setup battle UI
            this.showScreen('battleScreen');
            this.updateBattleDisplay();
            this.showBattleActions();
            this.updateBattleLog();

        } catch (error) {
            alert('Failed to start battle. Please try again.');
            console.error(error);
        }
    }

    /**     
     * Update the battle display with current Pokemon info
     */
    updateBattleDisplay() {
        if (!this.battle) return;

        const p1 = this.battle.activePokemon1;
        const p2 = this.battle.activePokemon2;

        // Update sprites
        document.getElementById('battlePlayer1Sprite').src = PokemonSprites.getSpriteUrl(p1.name);
        document.getElementById('battlePlayer2Sprite').src = PokemonSprites.getSpriteUrl(p2.name);

        // Update names
        document.getElementById('battlePlayer1Name').textContent = `${this.battle.player1.name}'s ${p1.name}`;
        document.getElementById('battlePlayer2Name').textContent = `${this.battle.player2.name}'s ${p2.name}`;

        // Update HP
        this.updateHealthBar(1, p1);
        this.updateHealthBar(2, p2);

        // Update attack options
        document.getElementById('normalAttackOption').textContent = `1. ${p1.normalAttack.name}`;
        document.getElementById('specialAttackOption').textContent = `2. ${p1.specialAttack.name}`;
    }

    /**     
     * Update health bar for a player
     * @param {number} playerNum 
     * @param {Pokemon} pokemon 
     */
    updateHealthBar(playerNum, pokemon) {
        const hpText = document.getElementById(`battlePlayer${playerNum}HP`);
        const hpBar = document.getElementById(`battlePlayer${playerNum}HealthBar`);

        const hpPercent = (pokemon.currentHitPoint / pokemon.maxHitPoint) * 100;
        
        hpText.textContent = `HP: ${pokemon.currentHitPoint}/${pokemon.maxHitPoint}`;
        hpBar.style.width = `${hpPercent}%`;

        // Color coding
        hpBar.classList.remove('low', 'critical');
        if (hpPercent < 20) {
            hpBar.classList.add('critical');
        } else if (hpPercent < 50) {
            hpBar.classList.add('low');
        }
    }

    /**     
     * Show battle action menu
     */
    showBattleActions() {
        document.getElementById('battleActionsMenu').classList.remove('hidden');
        document.getElementById('fightMenu').classList.add('hidden');
    }

    /**     
     * Show fight menu
     */
    showFightMenu() {
        document.getElementById('battleActionsMenu').classList.add('hidden');
        document.getElementById('fightMenu').classList.remove('hidden');
    }

    /**     
     * Execute an attack
     * @param {boolean} isSpecial 
     */
    executeAttack(isSpecial) {
        if (!this.battle) return;

        const result = this.battle.performAttack(isSpecial);
        
        if (result.success) {
            this.updateBattleDisplay();
            this.updateBattleLog();
            
            // Check if battle ended
            if (this.battle.getStatus() !== 'InProgress') {
                setTimeout(() => {
                    const winner = this.battle.getStatus() === 'Player1Won' ? this.battle.player1.name : this.battle.player2.name;
                    alert(`🏆 ${winner} wins the battle!`);
                    this.showScreen('mainMenu');
                }, 1000);
            } else {
                this.showBattleActions();
            }
        }
    }

    /**     
     * Update the battle log display
     */
    updateBattleLog() {
        const logContainer = document.getElementById('battleLogTerminal');
        const logs = this.battle.getBattleLog();

        logContainer.innerHTML = logs.map(log => 
            `<div class="log-entry ${log.type}">${log.message}</div>`
        ).join('');

        // Auto-scroll to bottom
        logContainer.scrollTop = logContainer.scrollHeight;
    }
}

// Initialize app when DOM is ready
document.addEventListener('DOMContentLoaded', () => new BattleApp());
