import { config } from '../config.js';

/**
 * Service for handling JWT authentication
 */
export class AuthService {
    constructor() {
        this.tokenKey = 'jwt_token';
        this.userKey = 'current_user';
        this.apiBaseUrl = config.apiBaseUrl;
    }

    /**
     * Login user and store JWT token
     * @param {string} userId - User ID to authenticate
     * @returns {Promise<Object>} Login response with token
     */
    async login(userId) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ userId })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || 'Login failed');
            }

            const data = await response.json();
            
            // Store token and user info
            localStorage.setItem(this.tokenKey, data.token);
            localStorage.setItem(this.userKey, JSON.stringify({
                userId: data.userId,
                username: data.username,
                expiresAt: data.expiresAt
            }));

            return data;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    }

    /**
     * Logout user and clear stored data
     */
    logout() {
        localStorage.removeItem(this.tokenKey);
        localStorage.removeItem(this.userKey);
    }

    /**
     * Get stored JWT token
     * @returns {string|null} JWT token or null
     */
    getToken() {
        return localStorage.getItem(this.tokenKey);
    }

    /**
     * Get current user info
     * @returns {Object|null} User info or null
     */
    getCurrentUser() {
        const userStr = localStorage.getItem(this.userKey);
        return userStr ? JSON.parse(userStr) : null;
    }

    /**
     * Check if user is authenticated
     * @returns {boolean} True if token exists and not expired
     */
    isAuthenticated() {
        const token = this.getToken();
        const user = this.getCurrentUser();

        if (!token || !user) {
            return false;
        }

        // Check if token is expired
        const expiresAt = new Date(user.expiresAt);
        if (expiresAt < new Date()) {
            this.logout();
            return false;
        }

        return true;
    }

    /**
     * Make authenticated API request
     * @param {string} url - API endpoint URL
     * @param {Object} options - Fetch options
     * @returns {Promise<Response>} Fetch response
     */
    async authenticatedFetch(url, options = {}) {
        const token = this.getToken();

        if (!token) {
            throw new Error('No authentication token found');
        }

        const headers = {
            ...options.headers,
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        const response = await fetch(url, {
            ...options,
            headers
        });

        // If unauthorized, logout and redirect
        if (response.status === 401) {
            this.logout();
            throw new Error('Session expired. Please login again.');
        }

        return response;
    }

    /**
     * Validate current token with server
     * @returns {Promise<boolean>} True if token is valid
     */
    async validateToken() {
        try {
            const response = await this.authenticatedFetch(`${this.apiBaseUrl}/auth/validate`);
            return response.ok;
        } catch (error) {
            console.error('Token validation error:', error);
            return false;
        }
    }
}
