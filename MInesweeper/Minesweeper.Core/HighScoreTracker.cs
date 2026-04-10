using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public class HighScoreTracker
    {
        /// <summary>
        /// string for the file path where high scores will be stored.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// new instance of the HighScoreTracker class, which takes an optional file path parameter. 
        /// If no file path is provided, it defaults to "data/HighScores.csv". 
        /// </summary>
        /// <param name="filePath"></param>
        public HighScoreTracker(string filePath = "data/HighScores.csv")
        {
            _filePath = filePath;
            EnsureFile();
        }

        /// <summary>
        /// ensures that there is a file at the specified path. 
        /// If the directory does not exist, it creates it. 
        /// If the file does not exist, it creates a new file with a header line.
        /// </summary>
        public void EnsureFile()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "size, seconds, moves, seed, timestamp\n");
            }
        }

        /// <summary>
        /// creates a list of HighScores objects by reading from the specified file path.
        /// includes a try-catch block to handle any exceptions that may occur during file reading/loading.
        /// </summary>
        /// <returns>scores that were loaded from the file</returns>
        public List<HighScores> Load()
        {
            var scores = new List<HighScores>();
            try
            {
                var lines = File.ReadAllLines(_filePath).Skip(1); // Skip header
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 5) continue;
                    if (!int.TryParse(parts[0], out int size)) continue;
                    if (!int.TryParse(parts[1], out int seconds)) continue;
                    if (!int.TryParse(parts[2], out int moves)) continue;
                    if (!int.TryParse(parts[3], out int seed)) continue;
                    if (!DateTime.TryParse(parts[4], out var ts)) continue;
                    {
                        scores.Add(new HighScores
                        {
                            Size = size,
                            Seconds = seconds,
                            Moves = moves,
                            Seed = seed,
                            Timestamp = ts
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading high scores: {ex.Message}");
            }
            return scores;
        }

        /// <summary>
        /// saves a list of HighScores objects to the specified file path.
        /// includes a try-catch block to handle any exceptions that may occur during file writing/saving.
        /// </summary>
        /// <param name="scores">The high scores to save</param>
        public void Save(List<HighScores> scores)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("size, seconds, moves, seed, timestamp");
                foreach (var s in scores)
                {
                    sb.AppendLine($"{s.Size}, {s.Seconds}, {s.Moves}, {s.Seed}, {s.Timestamp}");
                }
                File.WriteAllText(_filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving high scores: {ex.Message}");
            }
        }

        /// <summary>
        /// adds scores to the list of high scores and saves it to the file.
        /// </summary>
        /// <param name="score">The high score to add</param>
        public void AddScore(HighScores score)
        {
            var scores = Load();
            scores.Add(score);
            
            var filtered = scores
                .Where(s => s.Size == score.Size)
                .OrderBy(s => s.Seconds)
                .ThenBy(s => s.Moves)
                .Take(5)
                .ToList();

            var others = scores
                .Where(s => s.Size != score.Size)
                .ToList();
                others.AddRange(filtered);

            Save(others);
        }

        /// <summary>
        /// Retrieves the top high scores for a specific board size.
        /// </summary>
        /// <param name="size">The size of the board to filter high scores by.</param>
        /// <returns>An enumerable of the top high scores for the specified size.</returns>
        public IEnumerable<HighScores> GetTopForSize(int size) =>
            Load()
                .Where(s => s.Size == size)
                .OrderBy(s => s.Seconds)
                .ThenBy(s => s.Moves)
                .Take(5);
    }
}
