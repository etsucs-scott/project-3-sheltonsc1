using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public class Board
    {
        private readonly GamesConfig _config;
        private readonly Tiles[,] _tiles;
        public int Size => _config.Size;
        public Tiles this[int x, int y] => _tiles[x, y];

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

        public bool ToggleFlag(int x, int y)
        {
            if (!InBounds(x, y)) return false;
            var tile = _tiles[x, y];
            if (tile.IsRevealed) return false;
            tile.IsFlagged = !tile.IsFlagged;
            return true;
        }

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

        public bool InBounds(int x, int y) =>
            x >= 0 && x < Size && y >= 0 && y < Size;
    }
}
