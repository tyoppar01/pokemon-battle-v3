/**
 * User Handler - Manages user/trainer related operations
 */
import { UserUI } from '../components/index.js';

export class UserHandler {
    constructor(app) {
        this.app = app;
    }

    /**
     * Show the select players screen
     */
    async showSelectPlayersScreen() {
        await this.app.loadUsers();
        UserUI.showSelectPlayers();
    }

    /**
     * Show the create user screen
     */
    showCreateUserScreen() {
        this.app.selectedTeam = [];
        UserUI.showCreateUser();
    }

    /**
     * Show view users screen
     */
    async showViewUsersScreen() {
        console.log('showViewUsersScreen called');
        await this.app.loadUsers();
        console.log('Users loaded:', this.app.users);
        UserUI.showViewUsers(this.app.users);
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
        const user = this.app.users.find(u => u.id === userId);
        if (!user) return;
        
        const confirmed = confirm(`Are you sure you want to delete trainer "${user.name}"? This cannot be undone.`);
        if (!confirmed) return;
        
        try {
            await this.app.apiService.deleteUser(userId);
            this.app.soundManager.playConfirm();
            alert(`✓ Trainer "${user.name}" has been deleted!`);
            // Refresh the users list
            await this.showViewUsersScreen();
        } catch (error) {
            console.error('Failed to delete user:', error);
            this.app.soundManager.playError();
            alert('✗ Failed to delete trainer. Please try again.');
        }
    }

    /**
     * Show player preview when a trainer is selected
     * @param {number} playerNum 
     */
    async showPlayerPreview(playerNum) {
        const selectElement = document.getElementById(`player${playerNum}Select`);
        const userId = selectElement.value;
        
        if (!userId) {
            UserUI.showPlayerPreview(playerNum, null);
            return;
        }

        try {
            // Find user from the loaded users list
            const user = this.app.users.find(u => u.id === userId);
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
            this.app.selectedTeam = this.app.selectedTeam.filter(p => p !== pokemonName);
        } else {
            // Add to team (if not full)
            if (this.app.selectedTeam.length >= 6) {
                alert('Maximum 6 Pokemon allowed!');
                return;
            }
            element.classList.add('selected');
            this.app.selectedTeam.push(pokemonName);
        }

        UserUI.updateTeamDisplay(this.app.selectedTeam);
    }

    /**     
     * Create a new trainer with selected Pokemon
     */
    async createTrainer() {
        const name = document.getElementById('newTrainerName').value.trim();
        const gender = document.getElementById('newTrainerGender').value;

        if (!name) {
            this.app.soundManager.playError();
            alert('Please enter a trainer name!');
            return;
        }

        if (this.app.selectedTeam.length === 0) {
            this.app.soundManager.playError();
            alert('Please select at least one Pokemon!');
            return;
        }

        try {
            // Create user
            const user = await this.app.apiService.createUser(name, gender);
            
            // Add Pokemon to user
            for (const pokemonType of this.app.selectedTeam) {
                await this.app.apiService.addPokemonToUser(user.id, pokemonType);
            }

            this.app.soundManager.playConfirm();
            alert(`Trainer ${name} created successfully with ${this.app.selectedTeam.length} Pokemon!`);
            await this.app.loadUsers();
            const { MainMenuUI } = await import('../components/index.js');
            MainMenuUI.show();
        } catch (error) {
            this.app.soundManager.playError();
            alert('Failed to create trainer. Please try again.');
            console.error(error);
        }
    }
}
