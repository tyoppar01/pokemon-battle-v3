using System;
using System.Collections.Generic;
using System.Linq;
using PokemonBattle.Enums;
using PokemonBattle.Characters;
using PokemonBattle;
using PokemonBattle.Items;
using PokemonBattle.Services;
using PokemonBattle.Helpers;

namespace PokemonBattle.PokeBattle {

    public class PokeBattle {

        public Character Player1 { get; private set; }
        public Character Player2 { get; private set; }
        public Pokemon ActivePokemon1 { get; private set; }
        public Pokemon ActivePokemon2 { get; private set; }
        public BattleStatus Status { get; private set; }
        public int TurnCount { get; private set; }
        public int CurrentTurn { get; private set; }
        public List<string> BattleLog { get; private set; }

        public PokeBattle(Character player1, Character player2) {
            Player1 = player1;
            Player2 = player2;
            Status = BattleStatus.NotStarted;
            TurnCount = 0;
            CurrentTurn = 1;
            BattleLog = new List<string>();
        }

        public void StartBattle() {

            if (Player1.PokePockets.Count == 0 || Player2.PokePockets.Count == 0) {
                throw new InvalidOperationException("Both players must have at least one Pokemon to battle.");
            }

            ActivePokemon1 = Player1.PokePockets[0];
            ActivePokemon2 = Player2.PokePockets[0];
            Status = BattleStatus.InProgress;
            TurnCount = 1;
            
            LogAction($"Battle started! {Player1.Name}'s {ActivePokemon1.Type} vs {Player2.Name}'s {ActivePokemon2.Type}");
        }

        // ===================== Pokemon Methods ===================== //

        // Switch active Pokemon for a player
        public void SwitchPokemon(Character player, Pokemon newPokemon, bool changeTurn = false) {
            if (Status != BattleStatus.InProgress) throw new InvalidOperationException("Battle is not in progress.");
            if (!player.PokePockets.Contains(newPokemon)) throw new InvalidOperationException("Pokemon not found in player's pocket.");
            if (IsPokemonDefeated(newPokemon)) throw new InvalidOperationException("Cannot switch to a defeated Pokemon.");

            // swap to new active pokemon
            if (player == Player1) {
                ActivePokemon1 = newPokemon;
            } else {
                ActivePokemon2 = newPokemon;
            }

            LogAction($"{player.Name} switched to {newPokemon.Type}!");
            
            if (changeTurn) {
                SwitchTurn();
            }
        }

        // ===================== Item Methods ===================== //
        public void UseItem(Character player, Item item) {
            if (Status != BattleStatus.InProgress)
            {
                throw new InvalidOperationException("Battle is not in progress.");
            }

            if (!player.Inventory.Contains(item))
            {
                throw new InvalidOperationException("Item not found in player's inventory.");
            }

            if (item is Medicine medicine)
            {
                Pokemon activePokemon = player == Player1 ? ActivePokemon1 : ActivePokemon2;
                activePokemon.CurrentHitPoint += medicine.HealAmount;
                LogAction($"{player.Name} used {medicine.Name} to heal {activePokemon.Type} by {medicine.HealAmount} HP!");
            }
            else if (item is Pokeball pokeball)
            {
                Pokemon targetPokemon = player == Player1 ? ActivePokemon2 : ActivePokemon1;
                // AttemptCapture(player, targetPokemon, pokeball);
            }

            player.Inventory.RemoveItem(item);
            SwitchTurn();
        }

        // ===================== Damage Methods ===================== //  
        public void Attack(Pokemon attacker, Pokemon defender) {

            // Validation check only
            if (Status != BattleStatus.InProgress) throw new InvalidOperationException("Battle is not in progress.");

            // Calculate damage and apply to defender
            int damage = CalculateDamage(attacker, defender);
            defender.CurrentHitPoint -= damage;

            // Log attack action purpose
            string attackerOwner = attacker == ActivePokemon1 ? Player1.Name : Player2.Name;
            string defenderOwner = defender == ActivePokemon1 ? Player1.Name : Player2.Name;
            LogAction($"{attackerOwner}'s {attacker.Type} attacked {defenderOwner}'s {defender.Type} for {damage} damage!");

            // Check if defender is defeated
            if (IsPokemonDefeated(defender))
            {
                LogAction($"{defenderOwner}'s {defender.Type} was defeated!");
                CheckBattleEnd();
            }
            else
            {
                SwitchTurn();
            }
        }

        public int CalculateDamage(Pokemon attacker, Pokemon defender) {
            return (int) (attacker.Attack * (100.0 / (100 + defender.Defense)));
        }

        public void UseSpecialAttacks(Pokemon pokemon) {

            if (Status != BattleStatus.InProgress) { throw new InvalidOperationException("Battle is not in progress."); }

            Pokemon opponent = pokemon == ActivePokemon1 ? ActivePokemon2 : ActivePokemon1;
            string owner = pokemon == ActivePokemon1 ? Player1.Name : Player2.Name;

            // Get the special attack type from the Pokemon's SpecialAttack property
            PokemonType attackType = pokemon.SpecialAttack != null  ? Enum.Parse<PokemonType>(pokemon.SpecialAttack.Type) : pokemon.Type;

            // Calculate type effectiveness
            TypeEffectiveness effectiveness = TypeEffectivenessHelper.GetEffectiveness(attackType, opponent.Type);
            double typeMultiplier = TypeEffectivenessHelper.GetMultiplier(effectiveness);

            // Special skill does 1.5x damage with type effectiveness
            int baseDamage = CalculateDamage(pokemon, opponent);
            int specialDamage = (int)(baseDamage * 1.5 * typeMultiplier);
            opponent.CurrentHitPoint -= specialDamage;

            LogAction($"{owner}'s {pokemon.Type} used {pokemon.SpecialSkill} for {specialDamage} damage!");
            
            // Log type effectiveness if not normal
            string effectivenessMsg = TypeEffectivenessHelper.GetEffectivenessMessage(effectiveness);
            if (!string.IsNullOrEmpty(effectivenessMsg)) {
                LogAction(effectivenessMsg);
            }

            if (IsPokemonDefeated(opponent)) {
                string opponentOwner = opponent == ActivePokemon1 ? Player1.Name : Player2.Name;
                LogAction($"{opponentOwner}'s {opponent.Type} was defeated!");
                CheckBattleEnd();
            } else {
                SwitchTurn();
            }
        }

        // ================== Additional Methods ================== // 

        public void CheckBattleEnd() {
            
            // Check if Player 1's active Pokemon is defeated
            if (IsPokemonDefeated(ActivePokemon1)) {
                // Check if Player 1 has other alive Pokemon
                if (PokeTeamService.HasAlivePokemon(Player1.PokePockets)) {
                    LogAction($"{Player1.Name}'s active Pokemon was defeated but has other Pokemon available!");
                    // Don't end battle, allow switch - status remains InProgress
                } else {
                    Status = BattleStatus.Player2Won;
                    LogAction($"{Player2.Name} wins the battle!");
                }
            } 
            // Check if Player 2's active Pokemon is defeated
            else if (IsPokemonDefeated(ActivePokemon2)) {
                // Check if Player 2 has other alive Pokemon
                if (PokeTeamService.HasAlivePokemon(Player2.PokePockets)) {
                    LogAction($"{Player2.Name}'s active Pokemon was defeated but has other Pokemon available!");
                    // Don't end battle, allow switch - status remains InProgress
                } else {
                    Status = BattleStatus.Player1Won;
                    LogAction($"{Player1.Name} wins the battle!");
                }
            }
        }

        public Character GetWinner() {
            return Status switch {
                BattleStatus.Player1Won => Player1,
                BattleStatus.Player2Won => Player2,
                _ => null
            };
        }

        public bool IsPokemonDefeated(Pokemon pokemon) {
            return pokemon.CurrentHitPoint <= 0;
        }

        public List<string> GetBattleLog() {
            return new List<string>(BattleLog);
        }

        private void LogAction(string action) {
            BattleLog.Add($"Turn {TurnCount}: {action}");
        }

        private void SwitchTurn() {
            CurrentTurn = CurrentTurn == 1 ? 2 : 1;
            TurnCount++;
        }

    }
    
}
