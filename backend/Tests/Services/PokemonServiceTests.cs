using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using PokemonBattle.Services;
using PokemonBattle.Data;
using PokemonBattle.Data.Entities;

namespace PokemonBattle.Tests.Services {

    [TestFixture]
    public class PokemonServiceTests {
        private PokemonService _pokemonService;
        private PokemonDbContext _context;

        [SetUp]
        public void Setup() {
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PokemonDbContext(options);
            _pokemonService = new PokemonService(_context);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllPlayablePokemonAsync_ReturnsListOfPokemon() {
            // Act
            var result = await _pokemonService.GetAllPlayablePokemonAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetPlayablePokemonByNameAsync_ValidName_ReturnsPokemon() {
            // Act
            var result = await _pokemonService.GetPlayablePokemonByNameAsync("Pikachu");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Pikachu"));
            Assert.That(result.Type, Is.EqualTo("Electric"));
        }

        [Test]
        public async Task GetPlayablePokemonByNameAsync_InvalidName_ReturnsNull() {
            // Act
            var result = await _pokemonService.GetPlayablePokemonByNameAsync("InvalidPokemon");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPlayablePokemonByTypeAsync_ValidType_ReturnsFilteredList() {
            // Act
            var result = await _pokemonService.GetPlayablePokemonByTypeAsync("Fire");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.All(p => p.Type == "Fire"), Is.True);
        }

        [Test]
        public async Task GetAvailableTypesAsync_ReturnsUniqueTypes() {
            // Act
            var result = await _pokemonService.GetAvailableTypesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.Unique);
            Assert.That(result, Is.Ordered);
        }

        [Test]
        public async Task GetPokemonSortedByStatAsync_HP_ReturnsSortedByHP() {
            // Act
            var result = await _pokemonService.GetPokemonSortedByStatAsync("hp");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(1));
            Assert.That(result[0].BaseMaxHP, Is.GreaterThanOrEqualTo(result[^1].BaseMaxHP));
        }

        [Test]
        public async Task PokemonExistsAsync_ValidPokemon_ReturnsTrue() {
            // Act
            var result = await _pokemonService.PokemonExistsAsync("Charmander");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task PokemonExistsAsync_InvalidPokemon_ReturnsFalse() {
            // Act
            var result = await _pokemonService.PokemonExistsAsync("FakePokemon");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetPokemonCountAsync_ReturnsCorrectCount() {
            // Act
            var result = await _pokemonService.GetPokemonCountAsync();

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetPokemonStatisticsAsync_ReturnsStatistics() {
            // Act
            var result = await _pokemonService.GetPokemonStatisticsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("totalCount"));
            Assert.That(result.ContainsKey("typeDistribution"));
            Assert.That(result.ContainsKey("averageHP"));
            Assert.That(result.ContainsKey("strongestPokemon"));
            Assert.That(result["totalCount"], Is.GreaterThan(0));
        }
    }
}
