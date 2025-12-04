/**
 * API Integration Layer
 */
export { APIService };

class APIService {

    // Base URLs
    constructor() {
        this.baseURL = 'http://localhost:5000/api/user';
        this.pokemonURL = 'http://localhost:5000/api/pokemon';
    }

    /**
     * Get All Registered Users from backend
     * @returns {Promise<Array>} List of all users
     */
    async getAllUsers() {

        try {
            const response = await fetch(this.baseURL);
            if (!response.ok) throw new Error('Failed to fetch users');
            return await response.json();

        } catch (error) {
            console.error('Error fetching users:', error);
            throw error;
        }
    }

    /**
     * Get all Pokemon for a specific user
     * @param {*} userId 
     * @returns 
     */
    async getUserPokemon(userId) {

        try {
            const response = await fetch(`${this.baseURL}/${userId}/pokemon`);
            if (!response.ok) throw new Error('Failed to fetch Pokemon');
            return await response.json();

        } catch (error) {
            console.error('Error fetching Pokemon:', error);
            throw error;
        }
    }

    /**
     * Create a new user
     * @param {string} name 
     * @param {string} gender 
     * @returns {Promise<Object>} Created user object
     */
    async createUser(name, gender) {

        try {
            const response = await fetch(this.baseURL, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, gender })
            });

            if (!response.ok) throw new Error('Failed to create user');
            return await response.json();

        } catch (error) {
            console.error('Error creating user:', error);
            throw error;
        }
    }

    /**
     * Add a Pokemon to a specific user
     * @param {string} userId 
     * @param {string} pokemonType 
     * @returns {Promise<Object>} Added Pokemon object
     */
    async addPokemonToUser(userId, pokemonType) {

        try {
            const response = await fetch(`${this.baseURL}/${userId}/pokemon`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ pokemonType })
            });

            if (!response.ok) throw new Error('Failed to add Pokemon');
            return await response.json();

        } catch (error) {
            console.error('Error adding Pokemon:', error);
            throw error;
        }

    }

    /**
     * Get all playable Pokemon
     * @returns {Promise<Array>} List of all playable Pokemon
     */
    async getAllPlayablePokemon() {

        try {
            const response = await fetch(`${this.pokemonURL}/playable`);
            if (!response.ok) throw new Error('Failed to fetch playable Pokemon');
            const data = await response.json();
            return data.pokemon || data;

        } catch (error) {
            console.error('Error fetching playable Pokemon:', error);
            throw error;
        }
    }

    /**
     * Delete a user
     * @param {string} userId 
     * @returns {Promise<void>}
     */
    async deleteUser(userId) {

        try {
            const response = await fetch(`${this.baseURL}/${userId}`, {
                method: 'DELETE'
            });

            if (!response.ok) throw new Error('Failed to delete user');

        } catch (error) {
            console.error('Error deleting user:', error);
            throw error;
        }
    }

    /**
     * Update a user
     * @param {string} userId 
     * @param {string} name 
     * @param {string} gender 
     * @returns {Promise<Object>} Updated user object
     */
    async updateUser(userId, name, gender) {

        try {
            const response = await fetch(`${this.baseURL}/${userId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, gender })
            });

            if (!response.ok) throw new Error('Failed to update user');
            return await response.json();

        } catch (error) {
            console.error('Error updating user:', error);
            throw error;
        }
    }

}
