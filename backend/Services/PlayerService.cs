using System;
using System.Collections.Generic;
using System.Linq;
using PokemonBattle.Pokemons;
using PokemonBattle;
using PokemonBattle.PokeBattle;
using PokemonBattle.Enums;
using PokemonBattle.Characters;

namespace PokemonBattle.Services
{
    public class PlayerService
    {
        public Character Player { get; private set; }

        // Initialize player
        public void InitializePlayer(string playerName, List<Pokemon> pokeTeam)
        {
            Player = new Character(playerName, pokeTeam);
        }

        // Get player's Pokemon count
        public int GetPlayerPokemonCount()
        {
            return Player?.PokePockets.Count ?? 0;
        }
    
    }
}
