using System;
using System.Collections.Generic;
using System.Linq;
using PokemonBattle.Pokemons;
using PokemonBattle.Enums;

namespace PokemonBattle.Services
{
    public class PokeTeamService
    {
        private static Random random = new Random();

        // Starter Team
        public static readonly List<Pokemon> StarterTeam = new List<Pokemon> {
            new Pikachu("Thunder", 5), new Bulbasaur("Leaf", 5), new Charmander("Blaze", 5)
        };

        // Legendary team (Team Unfair)
        public static readonly List<Pokemon> LegendaryTeam = new List<Pokemon> {
            new Mewtwo("Alpha", 25), new Gengar("Omega", 22), new Snorlax("Titan", 20)
        };

        public static List<Pokemon> SelectTeam(List<Pokemon> team, bool isNew = true) {
            // Play with AI generated team
            if (isNew == true) { team = StarterTeam; }

            // Return Selected Team
            return team;
        }

        public static List<Pokemon> choosePokeTeam() 
        {
            // Create a NEW team list for each player
            var team = new List<Pokemon>();
            
            // Team of 6 Pokemons
            for (int i = 1; i <= 6; i++)
            {
                Console.WriteLine($"\nChoose Pokemon #{i} (or press 0 to finish team):");
                Console.Write("Enter Pokemon type number (1-8): ");
                
                if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0)
                {
                    if (team.Count > 0) break;
                    Console.WriteLine("You need at least one Pokemon! Please choose again.");
                    i--;
                    continue;
                }
                
                Console.Write("Enter a name for your Pokemon (Default name with enter): ");
                string pokemonName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(pokemonName))
                {
                    pokemonName = GetDefaultPokemonName(choice);
                }
                
                // Console.Write("Enter level (1-100): ");
                // if (!int.TryParse(Console.ReadLine(), out int level) || level < 1 || level > 100)
                // {
                //     level = 1;
                // }
                
                int level = 1;

                Pokemon newPokemon = choice switch
                {
                    1 => new Pikachu(pokemonName, level),
                    2 => new Charmander(pokemonName, level),
                    3 => new Squirtle(pokemonName, level),
                    4 => new Bulbasaur(pokemonName, level),
                    5 => new Gengar(pokemonName, level),
                    6 => new Mewtwo(pokemonName, level),
                    7 => new Snorlax(pokemonName, level),
                    8 => new Aron(pokemonName, level),
                    _ => new Pikachu(pokemonName, level)
                };
                
                team.Add(newPokemon);
                Console.WriteLine($"Added {newPokemon.Name} (Level {newPokemon.Level}) to your team!");
            }

            Console.WriteLine($"\nTeam created with {team.Count} Pokemon!");
            return team;
        }

        private static string GetDefaultPokemonName(int pokemonType)
        {
            return pokemonType switch
            {
                1 => "Pikachu",
                2 => "Charmander",
                3 => "Squirtle",
                4 => "Bulbasaur",
                5 => "Gengar",
                6 => "Mewtwo",
                7 => "Snorlax",
                8 => "Aron",
                _ => "Pokemon"
            };
        }

        // Get alive Pokemon from a team
        public static List<Pokemon> GetAlivePokemon(List<Pokemon> team)
        {
            return team?.Where(p => p.CurrentHitPoint > 0).ToList() ?? new List<Pokemon>();
        }

        // Get count of alive Pokemon
        public static int GetAlivePokemonCount(List<Pokemon> team)
        {
            return team?.Count(p => p.CurrentHitPoint > 0) ?? 0;
        }

        // Check if team has any alive Pokemon
        public static bool HasAlivePokemon(List<Pokemon> team)
        {
            return team?.Any(p => p.CurrentHitPoint > 0) ?? false;
        }

    }

}