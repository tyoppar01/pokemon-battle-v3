using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PokemonBattle.Characters;
using PokemonBattle.PokeBattle;

namespace PokemonBattle.Services {
    public class BattleLogger {
        private string _logDirectory;
        private string _currentLogFile;

        public BattleLogger(string logDirectory = "BattleLogs") {
            _logDirectory = logDirectory;
            
            // Create logs directory if it doesn't exist
            if (!Directory.Exists(_logDirectory)) {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public void SaveBattleResult(Character player1, Character player2, PokeBattle.PokeBattle battle) {
            // Generate unique filename with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _currentLogFile = Path.Combine(_logDirectory, $"Battle_{timestamp}.txt");

            var sb = new StringBuilder();
            
            // Battle header
            sb.AppendLine("=================================================");
            sb.AppendLine("           POKEMON BATTLE RESULT LOG            ");
            sb.AppendLine("=================================================");
            sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // Players information
            sb.AppendLine("--- PLAYERS ---");
            sb.AppendLine($"Player 1: {player1.Name}");
            sb.AppendLine($"  Team Size: {player1.PokePockets.Count} Pokemon");
            foreach (var pokemon in player1.PokePockets) {
                sb.AppendLine($"    - {pokemon.Name} ({pokemon.Type}) Level {pokemon.Level}");
                sb.AppendLine($"      HP: {pokemon.CurrentHitPoint}/{pokemon.MaxHitPoint} | Attack: {pokemon.Attack} | Defense: {pokemon.Defense}");
            }
            
            sb.AppendLine();
            sb.AppendLine($"Player 2: {player2.Name}");
            sb.AppendLine($"  Team Size: {player2.PokePockets.Count} Pokemon");
            foreach (var pokemon in player2.PokePockets) {
                sb.AppendLine($"    - {pokemon.Name} ({pokemon.Type}) Level {pokemon.Level}");
                sb.AppendLine($"      HP: {pokemon.CurrentHitPoint}/{pokemon.MaxHitPoint} | Attack: {pokemon.Attack} | Defense: {pokemon.Defense}");
            }
            
            sb.AppendLine();
            sb.AppendLine("--- BATTLE DETAILS ---");
            sb.AppendLine($"Status: {battle.Status}");
            sb.AppendLine($"Total Turns: {battle.TurnCount}");
            
            // Winner
            var winner = battle.GetWinner();
            if (winner != null) {
                sb.AppendLine($"Winner: {winner.Name}");
            }
            else {
                sb.AppendLine("Winner: Draw/No Winner");
            }
            
            sb.AppendLine();
            sb.AppendLine("--- BATTLE LOG ---");
            var battleLog = battle.GetBattleLog();
            foreach (var log in battleLog) {
                sb.AppendLine(log);
            }
            
            sb.AppendLine();
            sb.AppendLine("--- FINAL TEAM STATUS ---");
            sb.AppendLine($"{player1.Name}'s Team:");
            foreach (var pokemon in player1.PokePockets) {
                string status = pokemon.CurrentHitPoint > 0 ? "ALIVE" : "DEFEATED";
                sb.AppendLine($"  {pokemon.Name}: {status} (HP: {pokemon.CurrentHitPoint}/{pokemon.MaxHitPoint})");
            }
            
            sb.AppendLine();
            sb.AppendLine($"{player2.Name}'s Team:");
            foreach (var pokemon in player2.PokePockets) {
                string status = pokemon.CurrentHitPoint > 0 ? "ALIVE" : "DEFEATED";
                sb.AppendLine($"  {pokemon.Name}: {status} (HP: {pokemon.CurrentHitPoint}/{pokemon.MaxHitPoint})");
            }
            
            sb.AppendLine();
            sb.AppendLine("=================================================");
            sb.AppendLine("                  END OF LOG                     ");
            sb.AppendLine("=================================================");

            // Write to file
            File.WriteAllText(_currentLogFile, sb.ToString());
        }

        public string GetLastLogFilePath() {
            return _currentLogFile;
        }

        public List<string> GetAllLogFiles() {
            if (!Directory.Exists(_logDirectory)) {
                return new List<string>();
            }
            return new List<string>(Directory.GetFiles(_logDirectory, "Battle_*.txt"));
        }

        public string ReadLog(string logFilePath) {
            if (File.Exists(logFilePath)) {
                return File.ReadAllText(logFilePath);
            }
            return null;
        }

        public void DeleteLog(string logFilePath) {
            if (File.Exists(logFilePath)) {
                File.Delete(logFilePath);
            }
        }

        public void DeleteAllLogs() {
            if (Directory.Exists(_logDirectory)) {
                var files = Directory.GetFiles(_logDirectory, "Battle_*.txt");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        public int GetLogCount() {
            if (!Directory.Exists(_logDirectory)) {
                return 0;
            }
            return Directory.GetFiles(_logDirectory, "Battle_*.txt").Length;
        }

        // Generate a summary report of all battles
        public string GenerateSummaryReport() {
            var logFiles = GetAllLogFiles();
            if (logFiles.Count == 0) {
                return "No battle logs found.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("=================================================");
            sb.AppendLine("          BATTLE HISTORY SUMMARY                 ");
            sb.AppendLine("=================================================");
            sb.AppendLine($"Total Battles: {logFiles.Count}");
            sb.AppendLine();

            foreach (var logFile in logFiles) {
                string fileName = Path.GetFileName(logFile);
                sb.AppendLine($"- {fileName}");
            }

            sb.AppendLine("=================================================");
            return sb.ToString();
        }
    }
}
