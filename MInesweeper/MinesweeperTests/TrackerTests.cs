using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Core;

namespace MinesweeperTests
{
    public class TrackerTests
    {
        /// <summary>
        /// method to generate a temporary file path for testing purposes. 
        /// This ensures that each test runs with a clean slate and does not interfere with existing files or other tests. 
        /// </summary>
        /// <returns></returns>
        private string TempFile() =>
            Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv");

        /// <summary>
        /// checks for a file missing scenario. It creates a new HighScoreTracker instance with a temporary file path and attempts to load scores from it.
        /// </summary>
        [Fact]
        public void FileMissingFailsafe()
        {
            string path = TempFile();
            var tracker = new HighScoreTracker(path);

            var scores = tracker.Load();

            Assert.Empty(scores);
            Assert.True(File.Exists(path));
        }

        /// <summary>
        /// Checks if the tiebreaker logic correctly prioritizes fewer moves when the time is the same.
        /// </summary>
        [Fact]
        public void FewerMovesWinsTiebreaker()
        {
            string path = TempFile();
            var tracker = new HighScoreTracker(path);

            tracker.AddScore(new HighScores
            {
                Size = 8,
                Seconds = 60,
                Moves = 25,
                Seed = 123,
                Timestamp = DateTime.UtcNow
            });

            tracker.AddScore(new HighScores
            {
                Size = 8,
                Seconds = 60,
                Moves = 20, // Fewer moves should win the tiebreaker
                Seed = 123,
                Timestamp = DateTime.UtcNow
            });

            var topScores = tracker.GetTopForSize(8).ToList();
            Assert.Equal(20, topScores[0].Moves); // The second score with fewer moves should be ranked higher
            Assert.Equal(25, topScores[1].Moves); // The first score with more moves should be ranked lower
        }
    }
}
