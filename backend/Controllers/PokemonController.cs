using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PokemonBattle.Services;
using PokemonBattle.Services.Interfaces;
using PokemonBattle.DTOs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonBattle.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PokemonController : ControllerBase {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService) {
            _pokemonService = pokemonService;
        }

        /// <summary>
        /// Gets all playable Pokemon available in the roster
        /// </summary>
        /// <returns>List of all playable Pokemon with count</returns>
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
        /// Gets a specific playable Pokemon by name
        /// </summary>
        /// <param name="name">The name of the Pokemon to retrieve</param>
        /// <returns>Pokemon details if found, 404 if not found</returns>
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
        /// Gets all available Pokemon types in the roster
        /// </summary>
        /// <returns>List of Pokemon types with count</returns>
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
        /// Filters Pokemon by their type
        /// </summary>
        /// <param name="type">The Pokemon type to filter by</param>
        /// <returns>List of Pokemon matching the specified type, 404 if none found</returns>
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
        /// Gets Pokemon sorted by a specific stat (HP, Attack, Defense, Speed)
        /// </summary>
        /// <param name="stat">The stat name to sort by</param>
        /// <returns>List of Pokemon sorted by the specified stat in descending order</returns>
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
        /// Gets statistical summary of all Pokemon (averages, counts, etc.)
        /// </summary>
        /// <returns>Pokemon statistics including total count and average stats</returns>
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
        /// Checks if a Pokemon exists in the playable roster by name
        /// </summary>
        /// <param name="name">The name of the Pokemon to check</param>
        /// <returns>Boolean indicating whether the Pokemon exists</returns>
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
