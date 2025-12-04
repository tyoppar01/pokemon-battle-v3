using System;
using PokemonBattle.Services;
using static PokemonBattle.Consoler.Consoler;

namespace PokemonBattle {
    class Program {
        static void Main(string[] args) {

            // Show Pokemon Battle Logo
            ShowLogo();

            var multiplayer = new Multiplayer();

            // Start Multiplayer Game
            multiplayer.RunMultiplayerGame();

        }
    }
}
