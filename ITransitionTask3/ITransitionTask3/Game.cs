namespace NonTransitiveDice
{
    public class Game
    {
        private readonly List<Dice> _diceList;
        private readonly Dictionary<(Dice, Dice), double> _probabilities;

        public Game(List<Dice> diceList)
        {
            _diceList = diceList;
            _probabilities = ProbabilityCalculator.CalculateProbabilities(_diceList);
        }

        public void Start()
        {
            Console.WriteLine("Let's determine who makes the first move.");

            var firstMoveGen = new FairRandomGenerator(2); // 0 or 1
            Console.WriteLine($"I selected a random value in the range 0..1 (HMAC={firstMoveGen.Hmac}).");
            Console.WriteLine("Try to guess my selection.");
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine();
            if (input?.ToUpper() == "X") return;
            if (input == "?")
            {
                Console.WriteLine(ProbabilityCalculator.GetProbabilityTable(_probabilities));
                Console.Write("Your selection: ");
                input = Console.ReadLine();
            }

            if (!int.TryParse(input, out int userFirstChoice) || userFirstChoice < 0 || userFirstChoice > 1)
            {
                Console.WriteLine("Invalid choice. Please enter 0 or 1.");
                return;
            }

            int firstMoveResult = firstMoveGen.GetFairResult(userFirstChoice, 2);
            Console.WriteLine($"My selection: {firstMoveGen.ComputerChoice} (KEY={firstMoveGen.Key}).");

            bool userGoesFirst = firstMoveResult == 0;

            Dice userDice, computerDice;

            if (userGoesFirst)
            {
                Console.WriteLine("You make the first move and choose the dice.");
                userDice = SelectDice("Choose your dice:");
                computerDice = SelectComputerDice(userDice);
                Console.WriteLine($"I choose the {computerDice} dice.");
            }
            else
            {
                Console.WriteLine("I make the first move and choose the dice.");
                computerDice = SelectComputerDice();
                Console.WriteLine($"I choose the {computerDice} dice.");
                userDice = SelectDice("Choose your dice:");
            }

            Console.WriteLine($"You choose the {userDice} dice.");

            Console.WriteLine("It's time for my roll.");
            int computerRoll = PerformRoll(computerDice, "My roll result is {0}.");

            Console.WriteLine("It's time for your roll.");
            int userRoll = PerformRoll(userDice, "Your roll result is {0}.");

            Console.WriteLine($"You rolled {userRoll}, I rolled {computerRoll}.");
            if (userRoll > computerRoll)
            {
                Console.WriteLine("You win!");
            }
            else if (computerRoll > userRoll)
            {
                Console.WriteLine("I win!");
            }
            else
            {
                Console.WriteLine("It's a tie!");
            }
        }

        private Dice SelectDice(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                for (int i = 0; i < _diceList.Count; i++)
                {
                    Console.WriteLine($"{i} - {_diceList[i]}");
                }
                Console.WriteLine("X - exit");
                Console.WriteLine("? - help");
                Console.Write("Your selection: ");

                string input = Console.ReadLine();
                if (input?.ToUpper() == "X") Environment.Exit(0);
                if (input == "?")
                {
                    Console.WriteLine(ProbabilityCalculator.GetProbabilityTable(_probabilities));
                    continue;
                }

                if (int.TryParse(input, out int index) && index >= 0 && index < _diceList.Count)
                {
                    return _diceList[index];
                }

                Console.WriteLine($"Invalid selection. Please enter a number between 0 and {_diceList.Count - 1}.");
            }
        }

        private Dice SelectComputerDice(Dice excludedDice = null)
        {
            var availableDice = excludedDice == null
                ? _diceList
                : _diceList.Where(d => d != excludedDice).ToList();

            var bestDice = availableDice
                .OrderByDescending(d => _probabilities
                    .Where(kvp => kvp.Key.Item1 == d && availableDice.Contains(kvp.Key.Item2))
                    .Average(kvp => kvp.Value))
                .First();

            return bestDice;
        }

        private int PerformRoll(Dice dice, string resultMessage)
        {
            var rollGen = new FairRandomGenerator(dice.Faces.Length);
            Console.WriteLine($"I selected a random value in the range 0..{dice.Faces.Length - 1} (HMAC={rollGen.Hmac}).");
            Console.WriteLine($"Add your number modulo {dice.Faces.Length}.");

            for (int i = 0; i < dice.Faces.Length; i++)
            {
                Console.WriteLine($"{i} - {i}");
            }
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine();
            if (input?.ToUpper() == "X") Environment.Exit(0);
            if (input == "?")
            {
                Console.WriteLine(ProbabilityCalculator.GetProbabilityTable(_probabilities));
                Console.Write("Your selection: ");
                input = Console.ReadLine();
            }

            if (!int.TryParse(input, out int userChoice) || userChoice < 0 || userChoice >= dice.Faces.Length)
            {
                Console.WriteLine($"Invalid choice. Please enter a number between 0 and {dice.Faces.Length - 1}.");
                return PerformRoll(dice, resultMessage);
            }

            int fairResult = rollGen.GetFairResult(userChoice, dice.Faces.Length);
            Console.WriteLine($"My number is {rollGen.ComputerChoice} (KEY={rollGen.Key}).");
            Console.WriteLine($"The fair number generation result is {rollGen.ComputerChoice} + {userChoice} = {fairResult} (mod {dice.Faces.Length}).");

            int rollResult = dice.Roll(fairResult);
            Console.WriteLine(string.Format(resultMessage, rollResult));

            return rollResult;
        }
    }
}