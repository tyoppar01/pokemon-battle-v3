using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using PokemonBattle.Services;
using PokemonBattle.Data;
using PokemonBattle.Pokemons;

namespace PokemonBattle.Tests.Services {

    [TestFixture]
    public class UserServiceTests {
        private UserService _userService;
        private PokemonDbContext _context;

        [SetUp]
        public void Setup() {
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PokemonDbContext(options);
            _userService = new UserService(_context);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void CreateUser_ValidData_ReturnsUserId() {
            // Act
            var userId = _userService.CreateUser("TestUser", "Male");

            // Assert
            Assert.That(userId, Is.Not.Null);
            Assert.That(userId, Does.StartWith("user_"));
        }

        [Test]
        public void GetUser_ExistingUser_ReturnsCharacter() {
            // Arrange
            var userId = _userService.CreateUser("TestUser", "Female");

            // Act
            var user = _userService.GetUser(userId);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Name, Is.EqualTo("TestUser"));
            Assert.That(user.Gender, Is.EqualTo("Female"));
        }

        [Test]
        public void GetUser_NonExistentUser_ReturnsNull() {
            // Act
            var user = _userService.GetUser("nonexistent");

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void GetAllUsers_ReturnsAllUsers() {
            // Arrange
            _userService.CreateUser("User1", "Male");
            _userService.CreateUser("User2", "Female");

            // Act
            var users = _userService.GetAllUsers();

            // Assert
            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test]
        public void DeleteUser_ExistingUser_ReturnsTrue() {
            // Arrange
            var userId = _userService.CreateUser("TestUser");

            // Act
            var result = _userService.DeleteUser(userId);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_userService.GetUser(userId), Is.Null);
        }

        [Test]
        public void DeleteUser_NonExistentUser_ReturnsFalse() {
            // Act
            var result = _userService.DeleteUser("nonexistent");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateUser_ExistingUser_ReturnsTrue() {
            // Arrange
            var userId = _userService.CreateUser("OldName", "Male");

            // Act
            var result = _userService.UpdateUser(userId, "NewName", "Female");

            // Assert
            Assert.That(result, Is.True);
            var user = _userService.GetUser(userId);
            Assert.That(user.Name, Is.EqualTo("NewName"));
            Assert.That(user.Gender, Is.EqualTo("Female"));
        }

        [Test]
        public void UpdateUser_NonExistentUser_ReturnsFalse() {
            // Act
            var result = _userService.UpdateUser("nonexistent", "Name", "Male");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void AddPokemonToUser_ValidPokemon_ReturnsTrue() {
            // Arrange
            var userId = _userService.CreateUser("Trainer");
            var pokemon = PokemonFactory.CreatePokemon("Pikachu", "Pikachu", 5);

            // Act
            var result = _userService.AddPokemonToUser(userId, pokemon);

            // Assert
            Assert.That(result, Is.True);
            var userPokemon = _userService.GetUserPokemon(userId);
            Assert.That(userPokemon.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddPokemonToUser_NonExistentUser_ReturnsFalse() {
            // Arrange
            var pokemon = PokemonFactory.CreatePokemon("Pikachu", "Pikachu", 5);

            // Act
            var result = _userService.AddPokemonToUser("nonexistent", pokemon);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemovePokemonFromUser_ValidIndex_ReturnsTrue() {
            // Arrange
            var userId = _userService.CreateUser("Trainer");
            var pokemon = PokemonFactory.CreatePokemon("Pikachu", "Pikachu", 5);
            _userService.AddPokemonToUser(userId, pokemon);

            // Act
            var result = _userService.RemovePokemonFromUser(userId, 0);

            // Assert
            Assert.That(result, Is.True);
            var userPokemon = _userService.GetUserPokemon(userId);
            Assert.That(userPokemon.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemovePokemonFromUser_InvalidIndex_ReturnsFalse() {
            // Arrange
            var userId = _userService.CreateUser("Trainer");

            // Act
            var result = _userService.RemovePokemonFromUser(userId, 0);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetUserPokemon_ReturnsUserPokemonList() {
            // Arrange
            var userId = _userService.CreateUser("Trainer");
            var pokemon1 = PokemonFactory.CreatePokemon("Pikachu", "Pikachu", 5);
            var pokemon2 = PokemonFactory.CreatePokemon("Charmander", "Charmander", 5);
            _userService.AddPokemonToUser(userId, pokemon1);
            _userService.AddPokemonToUser(userId, pokemon2);

            // Act
            var userPokemon = _userService.GetUserPokemon(userId);

            // Assert
            Assert.That(userPokemon.Count, Is.EqualTo(2));
            Assert.That(userPokemon[0].Name, Is.EqualTo("Pikachu"));
            Assert.That(userPokemon[1].Name, Is.EqualTo("Charmander"));
        }
    }
}
