module.exports = {
  testEnvironment: 'jsdom',
  transform: {},
  moduleFileExtensions: ['js'],
  testMatch: ['**/__tests__/**/*.test.js'],
  collectCoverageFrom: [
    'js/**/*.js',
    '!js/main.js',
    '!js/config.js'
  ],
  setupFilesAfterEnv: ['<rootDir>/jest.setup.js']
};
