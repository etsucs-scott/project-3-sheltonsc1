using MInesweeper.Cli;
using Minesweeper.Core;
using System.Numerics;
using Microsoft.VisualBasic;



namespace MinesweeperTests
{
    public class BoardTests
    {
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

        [Fact]
        public void CheckAdjacentCount()
        {
            var config = new GamesConfig(8, 123);
            var board = new Board(config);
            
            var tile = board[0, 0];
            Assert.False(tile.IsMine);
            Assert.Equal(10, tile.AdjacentMines);
        }

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