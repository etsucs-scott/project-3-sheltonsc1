using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    /// <summary>
    /// data that is saved in the highscores.csv file, including size of the board, seconds taken to complete the game, number of moves taken,
    /// seed used to generate the board and a timestamp of when the game was completed
    /// (which is used for tie-breaking when there are multiple high scores with the same time and moves)
    /// </summary>
    public class HighScores
    {
        public int Size { get; set; }
        public int Seconds { get; set; }
        public int Moves { get; set; }
        public int Seed { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
