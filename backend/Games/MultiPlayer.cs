using System;
using PokemonBattle;
using PokemonBattle.Pokemons;
using PokemonBattle.PokeBattle;
using PokeBattle.Services;
using PokemonBattle.Characters;
using PokemonBattle.Services;
using PokemonBattle.Enums;
using PokemonBattle.Helpers;
using static PokemonBattle.Consoler.Consoler;

namespace PokemonBattle.Services {

    public class Multiplayer {
        
        public void RunMultiplayerGame() {

            // Initialize game service
            var gameService = new GameService();
            gameService.StartNewGame();

            // Player Setup Name and PokeTeam
            Character trainer1 = gameService.setupPlayer();
            Character trainer2 = gameService.setupPlayer("2");

            // Display player team summary
            TeamSummary(trainer1, trainer2);

            // Start the battle
            Console.WriteLine("\n=== STARTING BATTLE ===\n");
            var battle = new PokemonBattle.PokeBattle.PokeBattle(trainer1, trainer2);
            battle.StartBattle();

            // Battle Iteration
            while (battle.Status == PokemonBattle.Enums.BattleStatus.InProgress) {

                Console.WriteLine($"\n--- Turn {battle.TurnCount} ---");
                Character currentPlayer = battle.CurrentTurn == 1 ? trainer1 : trainer2;
                Pokemon attacker = battle.CurrentTurn == 1 ? battle.ActivePokemon1 : battle.ActivePokemon2;
                Pokemon defender = battle.CurrentTurn == 1 ? battle.ActivePokemon2 : battle.ActivePokemon1;
                
                // Check if current player's active Pokemon is defeated and force switch
                if (battle.IsPokemonDefeated(attacker)) {
                    Console.WriteLine($"\n⚠️  {currentPlayer.Name}'s {attacker.Name} has been defeated!");
                    
                    var alivePokemon = PokeTeamService.GetAlivePokemon(currentPlayer.PokePockets);
                    
                    if (alivePokemon.Count > 0) {
                        Console.WriteLine($"You must switch to another Pokemon!");
                        Pokemon selectedPokemon = ChooseAlivePokemon(currentPlayer);
                        
                        if (selectedPokemon != null) {
                            battle.SwitchPokemon(currentPlayer, selectedPokemon);
                            Console.WriteLine($"{currentPlayer.Name} sent out {selectedPokemon.Name}!");
                            
                            // Update attacker reference after switch
                            attacker = battle.CurrentTurn == 1 ? battle.ActivePokemon1 : battle.ActivePokemon2;
                        }
                    }
                    // If no alive Pokemon, CheckBattleEnd will end the game
                    else {
                        continue;
                    }
                }
                
                Console.WriteLine($"\n{currentPlayer.Name}'s Turn!");
                
                // Show current Pokemon status
                Console.WriteLine($"Your active Pokemon: {attacker.Name} (HP: {attacker.CurrentHitPoint}/{attacker.MaxHitPoint})");
                Console.WriteLine($"Opponent's Pokemon: {defender.Name} (HP: {defender.CurrentHitPoint}/{defender.MaxHitPoint})");
                
                // Player action loop - continues until valid action is taken
                bool actionTaken = false;
                while (!actionTaken) {
                    
                    // Show main battle menu
                    string mainChoice = ShowBattleMenu();
                    
                    switch (mainChoice) {
                        case "1": // FIGHT
                            string fightChoice = ShowFightMenu(attacker);
                            
                            switch (fightChoice?.ToUpper()) {
                                case "1": // Normal Attack
                                    Console.WriteLine($"\n{attacker.Name} uses {attacker.NormalAttack.Name}!");
                                    battle.Attack(attacker, defender);
                                    Console.WriteLine($"{defender.Name} HP: {defender.CurrentHitPoint}/{defender.MaxHitPoint}");
                                    
                                    // Check if defender is defeated after attack
                                    if (battle.IsPokemonDefeated(defender)) {
                                        HandleDefenderDefeated(battle, defender, trainer1, trainer2);
                                    }
                                    
                                    actionTaken = true;
                                    break;
                                
                                case "2": // Special Skill
                                    Console.WriteLine($"\n{attacker.Name} uses {attacker.SpecialAttack.Name}!");
                                    
                                    // Get type effectiveness before attack
                                    PokemonType attackType = attacker.SpecialAttack != null 
                                        ? Enum.Parse<PokemonType>(attacker.SpecialAttack.Type) 
                                        : attacker.Type;
                                    TypeEffectiveness effectiveness = TypeEffectivenessHelper.GetEffectiveness(attackType, defender.Type);
                                    
                                    battle.UseSpecialAttacks(attacker);
                                    Console.WriteLine($"{defender.Name} HP: {defender.CurrentHitPoint}/{defender.MaxHitPoint}");
                                    
                                    // Display effectiveness message
                                    string effectivenessMsg = TypeEffectivenessHelper.GetEffectivenessMessage(effectiveness);
                                    if (!string.IsNullOrEmpty(effectivenessMsg)) {
                                        Console.WriteLine($"💥 {effectivenessMsg}");
                                    }
                                    
                                    // Check if defender is defeated after special skill
                                    if (battle.IsPokemonDefeated(defender)) {
                                        HandleDefenderDefeated(battle, defender, trainer1, trainer2);
                                    }
                                    
                                    actionTaken = true;
                                    break;
                                
                                case "3":
                                case "4":
                                    Console.WriteLine("\nThis move is not available yet!");
                                    break;
                                
                                case "B":
                                    // Go back to main menu
                                    break;
                                
                                default:
                                    Console.WriteLine("\nInvalid choice! Please enter 1-4 or B.");
                                    break;
                            }
                            break;
                        
                        case "2": // POKEMON (Switch)
                            var alivePokemon = PokeTeamService.GetAlivePokemon(currentPlayer.PokePockets);
                            
                            if (alivePokemon.Count <= 1)
                            {
                                Console.WriteLine("\nYou have no other Pokemon to switch to!");
                            }
                            else
                            {
                                Pokemon selectedPokemon = ChooseAlivePokemon(currentPlayer);
                                
                                if (selectedPokemon != null && selectedPokemon != attacker)
                                {
                                    battle.SwitchPokemon(currentPlayer, selectedPokemon, true);
                                    actionTaken = true;
                                }
                                else if (selectedPokemon == attacker)
                                {
                                    Console.WriteLine("\nThat Pokemon is already in battle!");
                                }
                            }
                            break;
                            
                        default:
                            Console.WriteLine("\nInvalid choice! Please enter 1 or 2.");
                            break;
                    }
                }

                // Add a small delay for readability
                System.Threading.Thread.Sleep(1500);
            }

            // Announce winner
            Console.WriteLine("\n=== Battle Over! ===");
            var winner = battle.GetWinner();
            if (winner != null) {
                Console.WriteLine($"{winner.Name} wins the battle!");
            }

            // Show full battle log
            Console.WriteLine("\n--- Battle Log ---");
            foreach (var log in battle.GetBattleLog()) {
                Console.WriteLine(log);
            }

            // Save battle log to file
            var battleLogger = new BattleLogger();
            battleLogger.SaveBattleResult(trainer1, trainer2, battle);
            Console.WriteLine($"\n✓ Battle log saved to: {battleLogger.GetLastLogFilePath()}");
            Console.WriteLine($"Total battles logged: {battleLogger.GetLogCount()}");

            // End game
            gameService.EndGame();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private void HandleDefenderDefeated(PokemonBattle.PokeBattle.PokeBattle battle, Pokemon defender, Character trainer1, Character trainer2) {
            Character opponent = battle.CurrentTurn == 1 ? trainer2 : trainer1;
            Console.WriteLine($"\n⚠️  {opponent.Name}'s {defender.Name} has been defeated!");
            
            var opponentAlivePokemon = PokeTeamService.GetAlivePokemon(opponent.PokePockets);
            
            if (opponentAlivePokemon.Count > 0) {
                Console.WriteLine($"{opponent.Name} must switch to another Pokemon!");
                
                // Switch to opponent for Pokemon selection
                Console.WriteLine($"\n{opponent.Name}, choose your Pokemon:");
                Pokemon selectedPokemon = ChooseAlivePokemon(opponent);
                
                if (selectedPokemon != null) {
                    // Switch Pokemon AND change turn to the opponent
                    battle.SwitchPokemon(opponent, selectedPokemon, true);
                    Console.WriteLine($"{opponent.Name} sent out {selectedPokemon.Name}!");
                }
            }
        }

    }
}