namespace NonTransitiveDice
{

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var diceList = DiceParser.Parse(args);
                var game = new Game(diceList);
                game.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Usage: NonTransitiveDice.exe <dice1> <dice2> <dice3> ...");
                Console.WriteLine("Example: NonTransitiveDice.exe 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3");
            }
        }
    }
}