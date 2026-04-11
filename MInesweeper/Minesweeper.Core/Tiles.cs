using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    /// <summary>
    /// class of tiles on the board, with boolean values for whether the tile is a mine, revealed, or flagged, 
    /// and an integer value for the number of adjacent mines
    /// </summary>
    public class Tiles
    {
        public bool IsMine { get; set; }
        public int AdjacentMines { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
    }
}
