using Minesweeper.Core;
namespace MInesweeper.Cli

{
    internal class Program
    {
        static void Main(string[] args)
        {
            var hsTracker = new HighScoreTracker();

            while(true)
            {
                int size = GetBoardSize();
                int seed = GetSeedFromUser();
                var config = new GamesConfig(size, seed);
                var board = new Board(config);

                Console.WriteLine($"Starting game with {size} and seed {seed}");

                int moves = 0;
                var start = DateTime.UtcNow;
                bool gameOver = false;
                bool win = false;

                while (!gameOver)
                {
                    RenderBoard(board, revealMines: false);
                    Console.Write("Enter command (x row col, y row col, q): ");
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Invalid command. Please try again.");
                        continue;
                    }

                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var cmd = parts[0];

                    if (cmd == "q")
                    {
                        gameOver = true;
                        win = false;
                        break;
                    }

                    if (parts.Length != 3 || 
                        !int.TryParse(parts[1], out int x) || 
                        !int.TryParse(parts[2], out int y))
                    {
                        Console.WriteLine("Invalid command format. Please use 'x row col' or 'y row col'.");
                        continue;
                    }

                    if (x < 0 || x >= size || y < 0 || y >= size)
                    {
                        Console.WriteLine("Coordinates out of bounds. Please try again.");
                        continue;
                    }

                    bool changed = false;
                    if (cmd == "x")
                    {
                        bool hitMine;
                        changed = board.Reveal(x, y, out hitMine);
                        if (changed) moves++;
                        if (hitMine)
                        {
                            gameOver = true;
                            win = false;
                            Console.WriteLine("You hit a mine! Game over.");
                        }
                        else if (board.AllNonMinesRevealed())
                        {
                            gameOver = true;
                            win = true;
                        }
                    }
                    else if (cmd == "f")
                    {
                        changed = board.ToggleFlag(x, y);
                        if (changed) moves++;
                    }
                    else
                    {
                        Console.WriteLine("Unknown command. Please use 'x' to reveal or 'y' to flag.");
                    }

                    var elapsed = (int)(DateTime.UtcNow - start).TotalSeconds;
                    RenderBoard(board, revealMines: true);

                    if (win)
                    {
                        Console.WriteLine($"You Win! :D Time: {elapsed} seconds, Moves: {moves} Seed: {seed}");
                        hsTracker.AddScore(new HighScores 
                        { 
                            Size = size, 
                            Seconds = elapsed, 
                            Moves = moves, 
                            Seed = seed,
                            Timestamp = DateTime.UtcNow
                        });

                        Console.WriteLine("High Scores:");
                        foreach (var s in hsTracker.GetTopForSize(size))
                        {
                            Console.WriteLine($"{s.Seconds}s, {s.Moves} moves, seed {s.Seed}, {s.Timestamp}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Game Over.");
                    }

                    Console.WriteLine("Returning to main menu\n");
                }
            }
        }

        static int GetSeedFromUser()
        {
            Console.Write("Enter a seed (or leave blank for time-based): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return Environment.TickCount;
            }

            if (int.TryParse(input, out int seed))
            {
                return seed;
            }

            Console.WriteLine("Invalid seed. Using time-based seed.");
            return Environment.TickCount;
        }

        static int GetBoardSize()
        {
            while(true)
            {
                Console.WriteLine("Choose a board size: ");
                Console.WriteLine("1. Small (8x8)");
                Console.WriteLine("2. Medium (12x12)");
                Console.WriteLine("3. Large (16x16)");
                Console.Write("Enter choice (1-3): ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        return 8;
                    case "2":
                        return 12;
                    case "3":
                        return 16;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }
            }
        }

        static void RenderBoard(Board board, bool revealMines)
        {
            int size = board.Size;
            Console.Write("   ");
            for (int y = 0; y < size; y++)
            {
                Console.Write($"{y,2}");
            }
            Console.WriteLine();

            for (int x = 0; x < size; x++)
            {
                Console.Write($"{x,2} ");
                for (int y = 0; y < size; y++)
                {
                    var t = board[x, y];
                    char ch;
                    if (t.IsRevealed)
                    {
                        if (t.IsMine)
                        {
                            ch = 'b';
                        }
                        else if (t.AdjacentMines == 0)
                        {
                            ch = '.';
                        }
                        else
                        {
                            ch = t.AdjacentMines.ToString()[0];
                        }
                    }
                    else if (t.IsFlagged)
                    {
                        ch = 'f';
                    }
                    else if (revealMines && t.IsMine)
                    {
                        ch = 'b';
                    }
                    else
                    {
                        ch = '#';
                    }
                    Console.Write($" {ch}");
                }
                Console.WriteLine();
            }
        }
    }
}