using Microsoft.AspNetCore.Mvc;
using PokemonBattle.Services;
using PokemonBattle.DTOs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonBattle.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase {
        private readonly PokemonService _pokemonService;

        public PokemonController(PokemonService pokemonService) {
            _pokemonService = pokemonService;
        }

        /// <summary>
        /// Get all playable Pokemon with their base stats and information
        /// </summary>
        /// <returns>List of all available Pokemon</returns>
        [HttpGet("playable")]
        public async Task<IActionResult> GetAllPlayablePokemon() {
            try {
                var pokemon = await _pokemonService.GetAllPlayablePokemonAsync();
                return Ok(new { 
                    count = pokemon.Count,
                    pokemon 
                });
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to retrieve Pokemon", message = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed information about a specific playable Pokemon
        /// </summary>
        /// <param name="name">Pokemon name (e.g., Pikachu, Charmander)</param>
        /// <returns>Detailed Pokemon information</returns>
        [HttpGet("playable/{name}")]
        public async Task<IActionResult> GetPlayablePokemonByName(string name) {
            try {
                var pokemon = await _pokemonService.GetPlayablePokemonByNameAsync(name);

                if (pokemon == null) {
                    return NotFound(new { error = $"Pokemon '{name}' not found in playable roster" });
                }

                return Ok(pokemon);
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to retrieve Pokemon", message = ex.Message });
            }
        }

        /// <summary>
        /// Get list of available Pokemon types (simple list)
        /// </summary>
        /// <returns>List of Pokemon type names</returns>
        [HttpGet("types")]
        public async Task<IActionResult> GetAvailablePokemonTypes() {
            try {
                var types = await _pokemonService.GetAvailableTypesAsync();
                return Ok(new { types, count = types.Count });
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to retrieve types", message = ex.Message });
            }
        }

        /// <summary>
        /// Get Pokemon filtered by type
        /// </summary>
        /// <param name="type">Pokemon type (e.g., Electric, Fire, Water)</param>
        /// <returns>List of Pokemon matching the type</returns>
        [HttpGet("filter/type/{type}")]
        public async Task<IActionResult> GetPokemonByType(string type) {
            try {
                var pokemon = await _pokemonService.GetPlayablePokemonByTypeAsync(type);

                if (!pokemon.Any()) {
                    return NotFound(new { error = $"No Pokemon found with type '{type}'" });
                }

                return Ok(new {
                    type,
                    count = pokemon.Count,
                    pokemon
                });
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to filter Pokemon", message = ex.Message });
            }
        }

        /// <summary>
        /// Get Pokemon stats comparison (all Pokemon sorted by a specific stat)
        /// </summary>
        /// <param name="stat">Stat to sort by: hp, attack, defense, or speed</param>
        /// <returns>Pokemon sorted by specified stat</returns>
        [HttpGet("stats/{stat}")]
        public async Task<IActionResult> GetPokemonByStats(string stat) {
            try {
                var pokemon = await _pokemonService.GetPokemonSortedByStatAsync(stat);

                return Ok(new { 
                    sortedBy = stat, 
                    count = pokemon.Count,
                    pokemon 
                });
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to sort Pokemon", message = ex.Message });
            }
        }

        /// <summary>
        /// Get Pokemon statistics summary
        /// </summary>
        /// <returns>Summary statistics about all Pokemon</returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetPokemonStatistics() {
            try {
                var stats = await _pokemonService.GetPokemonStatisticsAsync();
                return Ok(stats);
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to retrieve statistics", message = ex.Message });
            }
        }

        /// <summary>
        /// Check if a Pokemon exists
        /// </summary>
        /// <param name="name">Pokemon name</param>
        /// <returns>Boolean indicating if Pokemon exists</returns>
        [HttpGet("exists/{name}")]
        public async Task<IActionResult> PokemonExists(string name) {
            try {
                var exists = await _pokemonService.PokemonExistsAsync(name);
                return Ok(new { name, exists });
            } catch (Exception ex) {
                return StatusCode(500, new { error = "Failed to check Pokemon existence", message = ex.Message });
            }
        }
    }

}
