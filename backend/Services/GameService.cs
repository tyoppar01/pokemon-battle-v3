
using System;
using System.Collections.Generic;
using PokemonBattle.Characters;
using PokemonBattle;
using PokemonBattle.Services;
using static PokemonBattle.Consoler.Consoler;

namespace PokeBattle.Services
{
    public class GameService
    {
        public bool IsGameActive { get; private set; }
        public List<Character> Players { get; private set; } = new List<Character>();

        public void StartNewGame() { IsGameActive = true; }

        public void EndGame() { IsGameActive = false; }

        /*
        Create a new player and add to the game
        */
        public void CreatePlayer(string PlayerName, List<Pokemon> pokeTeam) {
            var newPlayer = new Character(PlayerName, pokeTeam);
            Players.Add(newPlayer);
        }

        /*
        Create a Pokemon team for the player
        */
        public List<Pokemon> createPokeTeam() {
            // console print pokemon choices
            ShowPokemonSelection();

            // let user select up to 6 pokemon
            List<Pokemon> pokeTeam = PokeTeamService.choosePokeTeam();

            return pokeTeam;
        }

        /*
        Create the player and its PokeTeam
        */
        public Character setupPlayer(string playerNumber = "1") {
            Console.WriteLine($"=== PLAYER {playerNumber} SETUP ===");
            Console.Write("Enter your player name: ");
            string playerName = Console.ReadLine() ?? "ASH";
            Console.WriteLine("\nNow let's create your Pokemon team!");
            List<Pokemon> pokeTeam = createPokeTeam();
            var player = new Character(playerName, pokeTeam);
            return player;
        }

    }
}