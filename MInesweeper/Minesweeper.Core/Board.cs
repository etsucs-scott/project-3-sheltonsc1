using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public class Board
    {
        /// <summary>
        /// readonly fields for the created of the board, including the configuration of the game and a 2D array of tiles.
        /// </summary>
        private readonly GamesConfig _config;
        private readonly Tiles[,] _tiles;
        public int Size => _config.Size;
        public Tiles this[int x, int y] => _tiles[x, y];

        /// <summary>
        /// new instance of the Board class, which takes a GamesConfig object as a parameter and creates a 2D array of Tiles based on the size specified in the configuration.
        /// </summary>
        /// <param name="config"></param>
        public Board(GamesConfig config)
        {
            _config = config;
            _tiles = new Tiles[config.Size, config.Size];
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    _tiles[x, y] = new Tiles();
                }
            }
            PlaceMines();
            CalculateAdjacent();
        }

        /// <summary>
        /// method for "placing mines" on the board. 
        /// uses a random number gen with the seed specified in the config to ensure reproducibility.
        /// </summary>
        public void PlaceMines()
        {
            var rand = new Random(_config.Seed);
            int placed = 0;
            while (placed < _config.Mines)
            {
                int x = rand.Next(Size);
                int y = rand.Next(Size);
                if (_tiles[x, y].IsMine) continue;
                {
                    _tiles[x, y].IsMine = true;
                    placed++;
                }
            }
        }

        /// <summary>
        /// method for calculating the number of adjacent mines for each tile on the board.
        /// </summary>
        public void CalculateAdjacent()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (_tiles[x, y].IsMine) continue;
                    int count = 0;
                    ForEachNeighbor(x, y, (nx, ny) =>
                    {
                        if (_tiles[nx, ny].IsMine) count++;
                    });
                }
            }
        }

        /// <summary>
        /// method for iterating/scanning over the neighbors of a given tile (x, y) and performing an action on each neighbor's coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile</param>
        /// <param name="y">The y-coordinate of the tile</param>
        /// <param name="action">The action to perform on each neighbor's coordinates</param>
        public void ForEachNeighbor(int x, int y, Action<int, int> action)
        {
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < Size && ny >= 0 && ny < Size)
                {
                    action(nx, ny);
                }
            }
        }

        /// <summary>
        /// method for revealing a tile at the specified coordinates (x, y).
        /// </summary>
        /// <param name="x">The x-coordinate of the tile</param>
        /// <param name="y">The y-coordinate of the tile</param>
        /// <param name="hitMine">Output parameter indicating if a mine was hit</param>
        /// <returns>True if the tile was successfully revealed, false otherwise</returns>
        public bool Reveal(int x, int y, out bool hitMine)
        {
            hitMine = false;
            if (!InBounds(x, y)) return false;
            var tile = _tiles[x, y];
            if (tile.IsRevealed || tile.IsFlagged) return false;

            tile.IsRevealed = true;
            if (tile.IsMine)
            {
                hitMine = true;
                return true;
            }

            if (tile.AdjacentMines == 0)
            {
                CascadeReveal(x, y);
            }

            return true;
        }

        /// <summary>
        /// method for revealing all connected tiles with zero adjacent mines, starting from the specified coordinates (x, y)
        /// thus creating a "cascade"
        /// </summary>
        /// <param name="x">The x-coordinate of the starting tile</param>
        /// <param name="y">The y-coordinate of the starting tile</param>
        public void CascadeReveal(int x, int y)
        {
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((x, y));

            while (queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();
                ForEachNeighbor(cx, cy, (nx, ny) =>
                {
                    var t = _tiles[nx, ny];
                    if (t.IsRevealed || t.IsFlagged || t.IsMine) return;
                    t.IsRevealed = true;
                    if (t.AdjacentMines == 0)
                    {
                        queue.Enqueue((nx, ny));
                    }
                });
            }
        }

        /// <summary>
        /// method for toggling a flag on a tile at the specified coordinates (x, y).
        /// </summary>
        /// <param name="x">The x-coordinate of the tile</param>
        /// <param name="y">The y-coordinate of the tile</param>
        /// <returns>True if the flag was successfully toggled, false otherwise</returns>
        public bool ToggleFlag(int x, int y)
        {
            if (!InBounds(x, y)) return false;
            var tile = _tiles[x, y];
            if (tile.IsRevealed) return false;
            tile.IsFlagged = !tile.IsFlagged;
            return true;
        }

        /// <summary>
        /// method for checking if all non-mine tiles have been revealed, which would indicate a win condition in the game.
        /// </summary>
        /// <returns>True if all non-mine tiles have been revealed, false otherwise</returns>
        public bool AllNonMinesRevealed()
        {
            for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
            {
                var t = _tiles[x, y];
                if (!t.IsMine && !t.IsRevealed) return false;
            }
            return true;
        }

        /// <summary>
        /// method for checking if entered coords are in bounds of the board 
        /// </summary>
        /// <param name="x">The x-coordinate to check</param>
        /// <param name="y">The y-coordinate to check</param>
        /// <returns>True if the coordinates are within the bounds of the board, false otherwise</returns>
        public bool InBounds(int x, int y) =>
            x >= 0 && x < Size && y >= 0 && y < Size;
    }
}
