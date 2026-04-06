namespace Minesweeper.Core
{
    public class GamesConfig
    {
        public int Size { get; }
        public int Mines { get; }
        public int Seed { get; }

        public GamesConfig(int size, int seed)
        {
            Size = size;
            Mines = size switch
            {
                8 => 10,
                12 => 25,
                16 => 40,
                _ => throw new ArgumentException("Invalid size. Allowed values are 8, 12, or 16.")
            };
            Seed = seed;
        }
    }
}
