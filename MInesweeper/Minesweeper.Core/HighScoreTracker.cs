using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public class HighScoreTracker
    {
        private readonly string _filePath;

        public HighScoreTracker(string filePath = "data/HighScores.csv")
        {
            _filePath = filePath;
            EnsureFile();
        }

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

        public IEnumerable<HighScores> GetTopForSize(int size) =>
            Load()
                .Where(s => s.Size == size)
                .OrderBy(s => s.Seconds)
                .ThenBy(s => s.Moves)
                .Take(5);
    }
}
