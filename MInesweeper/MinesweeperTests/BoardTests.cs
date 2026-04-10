using MInesweeper.Cli;
using Minesweeper.Core;
using System.Numerics;
using Microsoft.VisualBasic;



namespace MinesweeperTests
{
    public class BoardTests
    {
        // seed 123 is an example of a fixed seed and is not supposed to be taken into account when evaluating the tests,
        // as the actual mine placement may differ based on the implementation of the random number generator. 
        // this applies to the tracker tests as well

        /// <summary>
        /// Checks if the board has the correct number of mines.
        /// </summary>
        [Fact]
        public void CheckMineCount()
        {
            var config = new GamesConfig(10, seed: 123);
            var board = new Board(config);

            int mines = 0;
            for (int x = 0; x < board.Size; x++)
            for (int y = 0; y < board.Size; y++)
            {
                if (board[x, y].IsMine) mines++;
            }
            Assert.Equal(10, mines);
        }

        /// <summary>
        /// Checks if the board correctly calculates the number of adjacent mines for each tile.
        /// </summary>
        [Fact]
        public void CheckAdjacentCount()
        {
            var config = new GamesConfig(8, 123);
            var board = new Board(config);
            
            var tile = board[0, 0];
            Assert.False(tile.IsMine);
            Assert.Equal(10, tile.AdjacentMines);
        }

        /// <summary>
        /// Checks if revealing a mine results in a hit
        /// and that the game logic correctly identifies it as a loss condition.
        /// </summary>
        [Fact]
        public void RevealHitMine()
        {
            var config = new GamesConfig(8, 123);
            var board = new Board(config);

            int mx = -1, my = -1;
            for (int x = 0; x < board.Size; x++)
            for (int y = 0; y < board.Size; y++)
            {
                if (board[x, y].IsMine)
                {
                    mx = x;
                    my = y;
                    break;
                }
            }
            bool hit;
            board.Reveal(mx, my, out hit);
            Assert.True(hit);
        }
        
        /// <summary>
        /// Checks if revealing an empty tile correctly reveals adjacent tiles.
        /// </summary>
        [Fact]
        public void RevealEmptyTile()
        {
            var config = new GamesConfig(8, 123);
            var board = new Board(config);

            int zx = -1, zy = -1;
            for (int x = 0; x < board.Size; x++)
            for (int y = 0; y < board.Size; y++)
            {
                if (!board[x, y].IsMine && board[x, y].AdjacentMines == 0)
                {
                    zx = x;
                    zy = y;
                    break;
                }
            }
            bool hit;
            board.Reveal(zx, zy, out hit);
            Assert.False(hit);

            bool anyAdjacentRevealed = false;
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = zx + dx, ny = zy + dy;
                if (nx < 0 || nx >= board.Size || ny < 0 || ny >= board.Size) continue;
                if (board[nx, ny].IsRevealed) anyAdjacentRevealed = true;
            }
            Assert.True(anyAdjacentRevealed);
        }

        /// <summary>
        /// Checks if flagged tiles cannot be revealed and that the game logic correctly 
        /// prevents revealing a flagged tile.
        /// </summary>
        [Fact]
        public void UnrevealableFlaggedTile()
        {
            var config = new GamesConfig(8, 123);
            var board = new Board(config);

            board.ToggleFlag(0, 0);
            bool hit;
            var result = board.Reveal(0, 0, out hit);

            Assert.False(result);
            Assert.False(hit);
            Assert.False(board[0, 0].IsRevealed);
        }

        /// <summary>
        /// Checks if all non-mine tiles are revealed amd that 
        /// the game logic correctly identifies it as a win condition.
        /// </summary>
        [Fact]
        public void NonMinesRevealed_WinCondition()
        {
            var config = new GamesConfig(8, 123);
            var board = new Board(config);

            for (int x = 0; x < board.Size; x++)
            for (int y = 0; y < board.Size; y++)
            {
                if (!board[x, y].IsMine)
                {
                    bool hit;
                    board.Reveal(x, y, out hit);
                }
            }

            Assert.True(board.AllNonMinesRevealed());
        }

    }
}