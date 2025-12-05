/**
 * Unit tests for AuthService
 */
import { jest, describe, test, expect, beforeEach, afterEach } from '@jest/globals';
import { AuthService } from '../services/authService.js';

describe('AuthService', () => {
  let authService;

  beforeEach(() => {
    authService = new AuthService();
    localStorage.clear();
    global.fetch = jest.fn();
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('constructor', () => {
    test('should initialize with correct properties', () => {
      expect(authService.tokenKey).toBe('jwt_token');
      expect(authService.userKey).toBe('current_user');
      expect(authService.apiBaseUrl).toBe('http://localhost:5000/api');
    });
  });

  describe('login', () => {
    test('should login successfully and store token', async () => {
      const mockResponse = {
        token: 'fake-jwt-token',
        userId: '123',
        username: 'Ash',
        expiresAt: new Date(Date.now() + 3600000).toISOString()
      };

      global.fetch.mockResolvedValue({
        ok: true,
        json: async () => mockResponse
      });

      const result = await authService.login('123');

      expect(result).toEqual(mockResponse);
      expect(localStorage.getItem('jwt_token')).toBe('fake-jwt-token');
      expect(JSON.parse(localStorage.getItem('current_user'))).toMatchObject({
        userId: '123',
        username: 'Ash'
      });
    });

    test('should throw error on failed login', async () => {
      global.fetch.mockResolvedValue({
        ok: false,
        json: async () => ({ error: 'User not found' })
      });

      await expect(authService.login('invalid-id')).rejects.toThrow('User not found');
    });
  });

  describe('logout', () => {
    test('should clear stored token and user data', () => {
      localStorage.setItem('jwt_token', 'token');
      localStorage.setItem('current_user', '{}');

      authService.logout();

      expect(localStorage.getItem('jwt_token')).toBeNull();
      expect(localStorage.getItem('current_user')).toBeNull();
    });
  });

  describe('getToken', () => {
    test('should return stored token', () => {
      localStorage.setItem('jwt_token', 'my-token');
      expect(authService.getToken()).toBe('my-token');
    });

    test('should return null if no token', () => {
      expect(authService.getToken()).toBeNull();
    });
  });

  describe('getCurrentUser', () => {
    test('should return parsed user object', () => {
      const user = { userId: '123', username: 'Ash' };
      localStorage.setItem('current_user', JSON.stringify(user));

      expect(authService.getCurrentUser()).toEqual(user);
    });

    test('should return null if no user', () => {
      expect(authService.getCurrentUser()).toBeNull();
    });
  });

  describe('isAuthenticated', () => {
    test('should return true for valid token', () => {
      localStorage.setItem('jwt_token', 'token');
      localStorage.setItem('current_user', JSON.stringify({
        userId: '123',
        expiresAt: new Date(Date.now() + 3600000).toISOString()
      }));

      expect(authService.isAuthenticated()).toBe(true);
    });

    test('should return false for expired token', () => {
      localStorage.setItem('jwt_token', 'token');
      localStorage.setItem('current_user', JSON.stringify({
        userId: '123',
        expiresAt: new Date(Date.now() - 1000).toISOString()
      }));

      expect(authService.isAuthenticated()).toBe(false);
      expect(localStorage.getItem('jwt_token')).toBeNull();
    });

    test('should return false if no token', () => {
      expect(authService.isAuthenticated()).toBe(false);
    });
  });

  describe('authenticatedFetch', () => {
    beforeEach(() => {
      localStorage.setItem('jwt_token', 'my-token');
    });

    test('should include Authorization header', async () => {
      global.fetch.mockResolvedValue({
        ok: true,
        status: 200
      });

      await authService.authenticatedFetch('http://api.example.com/data');

      expect(global.fetch).toHaveBeenCalledWith(
        'http://api.example.com/data',
        expect.objectContaining({
          headers: expect.objectContaining({
            'Authorization': 'Bearer my-token'
          })
        })
      );
    });

    test('should logout on 401 response', async () => {
      localStorage.setItem('current_user', '{}');
      global.fetch.mockResolvedValue({
        ok: false,
        status: 401
      });

      await expect(authService.authenticatedFetch('http://api.example.com/data'))
        .rejects.toThrow('Session expired. Please login again.');

      expect(localStorage.getItem('jwt_token')).toBeNull();
    });

    test('should throw error if no token', async () => {
      localStorage.clear();

      await expect(authService.authenticatedFetch('http://api.example.com/data'))
        .rejects.toThrow('No authentication token found');
    });
  });
});
