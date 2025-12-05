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
    mockApp = createMockApp();
    userHandler = new UserHandler(mockApp);
  });

  describe('constructor', () => {
    test('should initialize with app reference', () => {
      expect(userHandler.app).toBe(mockApp);
    });
  });

  describe('showSelectPlayersScreen', () => {
    test('should load users', async () => {
      await userHandler.showSelectPlayersScreen();
      expect(mockApp.loadUsers).toHaveBeenCalled();
    });
  });

  describe('showCreateUserScreen', () => {
    test('should reset selectedTeam', () => {
      mockApp.selectedTeam = ['Pikachu', 'Charmander'];
      userHandler.showCreateUserScreen();
      expect(mockApp.selectedTeam).toEqual([]);
    });
  });

  describe('togglePokemonSelection', () => {
    test('should add Pokemon to team when not selected', () => {
      const mockElement = {
        dataset: { pokemon: 'Pikachu' },
        classList: {
          contains: jest.fn(() => false),
          add: jest.fn(),
          remove: jest.fn()
        }
      };

      userHandler.togglePokemonSelection(mockElement);

      expect(mockApp.selectedTeam).toContain('Pikachu');
      expect(mockElement.classList.add).toHaveBeenCalledWith('selected');
    });

    test('should remove Pokemon from team when already selected', () => {
      mockApp.selectedTeam = ['Pikachu'];
      const mockElement = {
        dataset: { pokemon: 'Pikachu' },
        classList: {
          contains: jest.fn(() => true),
          add: jest.fn(),
          remove: jest.fn()
        }
      };

      userHandler.togglePokemonSelection(mockElement);

      expect(mockApp.selectedTeam).not.toContain('Pikachu');
      expect(mockElement.classList.remove).toHaveBeenCalledWith('selected');
    });

    test('should not add more than 6 Pokemon to team', () => {
      mockApp.selectedTeam = ['P1', 'P2', 'P3', 'P4', 'P5', 'P6'];
      global.alert = jest.fn();

      const mockElement = {
        dataset: { pokemon: 'Pikachu' },
        classList: {
          contains: jest.fn(() => false),
          add: jest.fn()
        }
      };

      userHandler.togglePokemonSelection(mockElement);

      expect(mockApp.selectedTeam).toHaveLength(6);
      expect(global.alert).toHaveBeenCalledWith('Maximum 6 Pokemon allowed!');
    });
  });

  describe('createTrainer', () => {
    beforeEach(() => {
      // Mock DOM elements
      global.document = {
        getElementById: jest.fn((id) => {
          if (id === 'newTrainerName') return { value: 'Ash' };
          if (id === 'newTrainerGender') return { value: 'Male' };
          return null;
        })
      };
      global.alert = jest.fn();
    });

    test('should create trainer with selected Pokemon', async () => {
      mockApp.selectedTeam = ['Pikachu', 'Charmander'];
      mockApp.apiService.createUser.mockResolvedValue({ id: '123', name: 'Ash' });
      mockApp.apiService.addPokemonToUser.mockResolvedValue({});

      await userHandler.createTrainer();

      expect(mockApp.apiService.createUser).toHaveBeenCalledWith('Ash', 'Male');
      expect(mockApp.apiService.addPokemonToUser).toHaveBeenCalledTimes(2);
      expect(mockApp.soundManager.playConfirm).toHaveBeenCalled();
    });

    test('should show error if name is empty', async () => {
      global.document.getElementById = jest.fn((id) => {
        if (id === 'newTrainerName') return { value: '  ' };
        if (id === 'newTrainerGender') return { value: 'Male' };
        return null;
      });

      await userHandler.createTrainer();

      expect(mockApp.soundManager.playError).toHaveBeenCalled();
      expect(global.alert).toHaveBeenCalledWith('Please enter a trainer name!');
      expect(mockApp.apiService.createUser).not.toHaveBeenCalled();
    });

    test('should show error if no Pokemon selected', async () => {
      mockApp.selectedTeam = [];

      await userHandler.createTrainer();

      expect(mockApp.soundManager.playError).toHaveBeenCalled();
      expect(global.alert).toHaveBeenCalledWith('Please select at least one Pokemon!');
      expect(mockApp.apiService.createUser).not.toHaveBeenCalled();
    });

    test('should handle creation error', async () => {
      mockApp.selectedTeam = ['Pikachu'];
      mockApp.apiService.createUser.mockRejectedValue(new Error('API Error'));

      await userHandler.createTrainer();

      expect(mockApp.soundManager.playError).toHaveBeenCalled();
      expect(global.alert).toHaveBeenCalledWith('Failed to create trainer. Please try again.');
    });
  });

  describe('deleteUser', () => {
    beforeEach(() => {
      global.confirm = jest.fn(() => true);
      global.alert = jest.fn();
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

    test('should handle deletion error', async () => {
      mockApp.apiService.deleteUser.mockRejectedValue(new Error('API Error'));

      await userHandler.deleteUser('123');

      expect(mockApp.soundManager.playError).toHaveBeenCalled();
      expect(global.alert).toHaveBeenCalledWith('✗ Failed to delete trainer. Please try again.');
    });
  });
});
