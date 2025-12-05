/**
 * Unit tests for UserHandler
 */
import { jest, describe, test, expect, beforeEach } from '@jest/globals';
import { UserHandler } from '../handlers/UserHandler.js';

// Mock the app object
const createMockApp = () => ({
  users: [],
  selectedTeam: [],
  apiService: {
    createUser: jest.fn(),
    deleteUser: jest.fn(),
    addPokemonToUser: jest.fn()
  },
  soundManager: {
    playConfirm: jest.fn(),
    playError: jest.fn()
  },
  loadUsers: jest.fn()
});

describe('UserHandler', () => {
  let userHandler;
  let mockApp;

  beforeEach(() => {
    jest.clearAllMocks();
    global.alert = jest.fn();
    global.confirm = jest.fn();

    mockApp = createMockApp();
    userHandler = new UserHandler(mockApp);
  });

  describe('constructor', () => {
    test('should initialize with app reference', () => {
      expect(userHandler.app).toBe(mockApp);
    });
  });

  describe('deleteUser', () => {
    beforeEach(() => {
      global.confirm = jest.fn(() => true);
      mockApp.users = [{ id: '123', name: 'Ash' }];
      userHandler.showViewUsersScreen = jest.fn();
    });

    test('should delete user after confirmation', async () => {
      mockApp.apiService.deleteUser.mockResolvedValue({});

      await userHandler.deleteUser('123');

      expect(global.confirm).toHaveBeenCalledWith(
        'Are you sure you want to delete trainer "Ash"? This cannot be undone.'
      );
      expect(mockApp.apiService.deleteUser).toHaveBeenCalledWith('123');
      expect(mockApp.soundManager.playConfirm).toHaveBeenCalled();
    });

    test('should not delete if user cancels', async () => {
      global.confirm = jest.fn(() => false);

      await userHandler.deleteUser('123');

      expect(mockApp.apiService.deleteUser).not.toHaveBeenCalled();
    });
  });
});
