using PokemonBattle.Characters;
using System;
using System.Collections.Generic;
using PokemonBattle.Pokemons;
using PokemonBattle.Services;

namespace PokemonBattle.Consoler {
    public static class Consoler {
        
        // Display Pokemon Battle ASCII logo
        public static void ShowLogo() {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
                                                                            ╔════════════════════════════════════════════════════════════════════╗
                                                                            ║                                                                    ║
                                                                            ║   ██████╗  ██████╗ ██╗  ██╗███████╗███╗   ███╗ ██████╗ ███╗   ██║  ║
                                                                            ║   ██╔══██╗██╔═══██╗██║ ██╔╝██╔════╝████╗ ████║██╔═══██╗████╗  ██║  ║
                                                                            ║   ██████╔╝██║   ██║█████╔╝ █████╗  ██╔████╔██║██║   ██║██╔██╗ ██║  ║
                                                                            ║   ██╔═══╝ ██║   ██║██╔═██╗ ██╔══╝  ██║╚██╔╝██║██║   ██║██║╚██╗██║  ║
                                                                            ║   ██║     ╚██████╔╝██║  ██╗███████╗██║ ╚═╝ ██║╚██████╔╝██║ ╚████║  ║
                                                                            ║   ╚═╝      ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝ ╚═════╝ ╚═╝  ╚═══╝  ║
                                                                            ║                                                                    ║
                                                                            ║   ██████╗  █████╗ ████████╗████████╗██╗     ███████╗               ║
                                                                            ║   ██╔══██╗██╔══██╗╚══██╔══╝╚══██╔══╝██║     ██╔════╝               ║
                                                                            ║   ██████╔╝███████║   ██║      ██║   ██║     █████╗                 ║
                                                                            ║   ██╔══██╗██╔══██║   ██║      ██║   ██║     ██╔══╝                 ║
                                                                            ║   ██████╔╝██║  ██║   ██║      ██║   ███████╗███████╗               ║
                                                                            ║   ╚═════╝ ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚══════╝╚══════╝               ║
                                                                            ║                                                                    ║
                                                                            ╚════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Display summary of both trainers and their Pokemon
        public static void TeamSummary(Character trainer1, Character trainer2) {

            Console.WriteLine("\n=== PLAYERS READY ===");
            Console.WriteLine($"Trainer 1: {trainer1.Name} with {trainer1.PokePockets.Count} Pokemon");

            foreach (var pokemon in trainer1.PokePockets) {
                Console.WriteLine($"  - {pokemon.Name} (Level {pokemon.Level}, HP: {pokemon.CurrentHitPoint}, Attack: {pokemon.Attack})");
            }
            
            Console.WriteLine($"\nTrainer 2: {trainer2.Name} with {trainer2.PokePockets.Count} Pokemon");
            foreach (var pokemon in trainer2.PokePockets) {
                Console.WriteLine($"  - {pokemon.Name} (Level {pokemon.Level}, HP: {pokemon.CurrentHitPoint}, Attack: {pokemon.Attack})");
            }
        }

        // Display available Pokemon for team selection
        public static void ShowPokemonSelection() {
            Console.WriteLine("Create your Pokemon team! You can choose up to 6 Pokemon.");
            Console.WriteLine("Available Pokemon types:");
            Console.WriteLine("1. Pikachu (Electric)");
            Console.WriteLine("2. Charmander (Fire)");
            Console.WriteLine("3. Squirtle (Water)");
            Console.WriteLine("4. Bulbasaur (Grass)");
            Console.WriteLine("5. Gengar (Ghost)");
            Console.WriteLine("6. Mewtwo (Psychic)");
            Console.WriteLine("7. Snorlax (Normal)");
            Console.WriteLine("8. Aron (Steel)");
        }

        // Display alive Pokemon and let user choose one
        public static Pokemon ChooseAlivePokemon(Character player) {

            // Get alive Pokemon using LINQ
            var alivePokemon = PokeTeamService.GetAlivePokemon(player.PokePockets);

            if (alivePokemon.Count == 0) {
                Console.WriteLine($"{player.Name} has no alive Pokemon!");
                return null;
            }

            Console.WriteLine($"\n=== {player.Name}'s Alive Pokemon ===");
            for (int i = 0; i < alivePokemon.Count; i++) {
                var pokemon = alivePokemon[i];
                Console.WriteLine($"{i + 1}. {pokemon.Name} ({pokemon.Type}) - Level {pokemon.Level}");
                Console.WriteLine($"   HP: {pokemon.CurrentHitPoint}/{pokemon.MaxHitPoint} | Attack: {pokemon.Attack} | Defense: {pokemon.Defense}");
            }

            while (true) {
                Console.Write($"\nChoose a Pokemon (1-{alivePokemon.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= alivePokemon.Count) {
                    var selectedPokemon = alivePokemon[choice - 1];
                    Console.WriteLine($"Selected: {selectedPokemon.Name}!");
                    return selectedPokemon;
                }
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }

        // Display alive Pokemon status (without choosing)
        public static void ShowAlivePokemon(Character player) {
            var alivePokemon = PokeTeamService.GetAlivePokemon(player.PokePockets);
            var aliveCount = PokeTeamService.GetAlivePokemonCount(player.PokePockets);

            Console.WriteLine($"\n{player.Name}'s Status: {aliveCount}/{player.PokePockets.Count} Pokemon alive");
            
            if (alivePokemon.Count == 0)
            {
                Console.WriteLine("  No Pokemon remaining!");
                return;
            }

            foreach (var pokemon in alivePokemon)
            {
                string healthBar = GenerateHealthBar(pokemon.CurrentHitPoint, pokemon.MaxHitPoint);
                Console.WriteLine($"  ✓ {pokemon.Name} ({pokemon.Type}) - HP: {pokemon.CurrentHitPoint}/{pokemon.MaxHitPoint} {healthBar}");
            }
        }

        // Display both players' alive Pokemon
        public static void ShowBothPlayersStatus(Character player1, Character player2) {
            Console.WriteLine("\n=== BATTLE STATUS ===");
            ShowAlivePokemon(player1);
            ShowAlivePokemon(player2);
        }

        // Helper method to generate a simple health bar
        private static string GenerateHealthBar(int currentHP, int maxHP) {
            int barLength = 10;
            double percentage = (double)currentHP / maxHP;
            int filledBars = (int)(percentage * barLength);
            
            if (filledBars < 0) filledBars = 0;
            if (filledBars > barLength) filledBars = barLength;

            string filled = new string('█', filledBars);
            string empty = new string('░', barLength - filledBars);
            
            return $"[{filled}{empty}]";
        }

        // Display battle action menu
        public static string ShowBattleMenu() {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║        BATTLE ACTIONS              ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.Write("║  ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("1. FIGHT");
            Console.Write("          ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("2. POKEMON");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("      ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.ResetColor();
            Console.Write("\nChoose action (1-2): ");
            return Console.ReadLine();
        }

        // Display fight sub-menu with 4 attack options
        public static string ShowFightMenu(Pokemon attacker) {

            var attackerName = attacker.NormalAttack?.Name ?? "Normal Attack";
            var specialAttackName = attacker.SpecialAttack?.Name ?? "Special Attack";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n╔══════════════════════════════════════╗");
            Console.WriteLine("║          FIGHT MENU                  ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            
            Console.Write("║  ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"1. {attackerName.PadRight(32)}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ║");
            
            Console.Write("║  ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"2. {specialAttackName.PadRight(32)}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ║");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("║  3. (Reserved Move)                  ║");
            Console.WriteLine("║  4. (Reserved Move)                  ║");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.ResetColor();
            
            Console.Write("║  ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("B. ← Back to Main Menu");
            Console.ResetColor();
            Console.WriteLine("              ║");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.Write("\nChoose attack (1-4, B): ");

            return Console.ReadLine();
        }

    }

}