using Microsoft.AspNetCore.Mvc;
using PokemonBattle.Services;
using PokemonBattle.DTOs;

namespace PokemonBattle.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase {
        private readonly UserService _userService;

        public UserController(UserService userService) {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request) {
            if (string.IsNullOrWhiteSpace(request.Name)) {
                return BadRequest(new { error = "Name is required" });
            }

            string userId = _userService.CreateUser(request.Name, request.Gender ?? "Unknown");
            var user = _userService.GetUser(userId);

            var response = UserResponse.FromCharacter(user, userId);
            return CreatedAtAction(nameof(GetUser), new { id = userId }, response);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(string id) {
            var user = _userService.GetUser(id);
            
            if (user == null) {
                return NotFound(new { error = "User not found" });
            }

            var response = UserResponse.FromCharacter(user, id);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAllUsers() {
            var users = _userService.GetAllUsers();
            var response = users.Select(kvp => UserResponse.FromCharacter(kvp.Value, kvp.Key)).ToList();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id) {
            bool deleted = _userService.DeleteUser(id);
            
            if (!deleted) {
                return NotFound(new { error = "User not found" });
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] CreateUserRequest request) {
            if (string.IsNullOrWhiteSpace(request.Name)) {
                return BadRequest(new { error = "Name is required" });
            }

            bool updated = _userService.UpdateUser(id, request.Name, request.Gender ?? "Unknown");
            
            if (!updated) {
                return NotFound(new { error = "User not found" });
            }

            var user = _userService.GetUser(id);
            var response = UserResponse.FromCharacter(user, id);
            return Ok(response);
        }

        // Pokemon Management Endpoints
        [HttpPost("{id}/pokemon")]
        public IActionResult AddPokemonToUser(string id, [FromBody] AddPokemonRequest request) {
            var user = _userService.GetUser(id);
            if (user == null) {
                return NotFound(new { error = "User not found" });
            }

            if (string.IsNullOrWhiteSpace(request.PokemonType)) {
                return BadRequest(new { error = "Pokemon type is required" });
            }

            try {
                var pokemon = PokemonFactory.CreatePokemon(
                    request.PokemonType, 
                    request.Name, 
                    request.Level
                );
                
                _userService.AddPokemonToUser(id, pokemon);
                
                var response = UserResponse.FromCharacter(user, id);
                return Ok(response);
            } catch (ArgumentException ex) {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}/pokemon/{pokemonIndex}")]
        public IActionResult RemovePokemonFromUser(string id, int pokemonIndex) {
            var user = _userService.GetUser(id);
            if (user == null) {
                return NotFound(new { error = "User not found" });
            }

            bool removed = _userService.RemovePokemonFromUser(id, pokemonIndex);
            if (!removed) {
                return BadRequest(new { error = "Invalid pokemon index" });
            }

            var response = UserResponse.FromCharacter(user, id);
            return Ok(response);
        }

        [HttpGet("{id}/pokemon")]
        public IActionResult GetUserPokemon(string id) {
            var pokemon = _userService.GetUserPokemon(id);
            
            if (pokemon == null) {
                return NotFound(new { error = "User not found" });
            }

            var response = pokemon.Select(p => PokemonDto.FromPokemon(p)).ToList();
            return Ok(response);
        }

        [HttpGet("pokemon/types")]
        public IActionResult GetAvailablePokemonTypes() {
            var types = PokemonFactory.GetAvailablePokemonTypes();
            return Ok(types);
        }
    }

}
