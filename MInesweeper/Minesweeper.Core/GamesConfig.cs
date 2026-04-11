namespace Minesweeper.Core
{
    public class GamesConfig
    {
        public int Size { get; }
        public int Mines { get; }
        public int Seed { get; }

        /// <summary>
        /// new instance of GamesConfig with the specified size and seed. 
        /// The number of mines is determined based on the size of the board
        /// </summary>
        /// <param name="size">length and width of the board</param>
        /// <param name="seed">seed for random number generation</param>
        /// <exception cref="ArgumentException">error message in case there is an invalid size entered</exception>
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
