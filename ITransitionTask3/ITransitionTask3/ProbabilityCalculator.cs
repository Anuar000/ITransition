using ConsoleTables;

namespace NonTransitiveDice
{
    public class ProbabilityCalculator
    {
        public static Dictionary<(Dice, Dice), double> CalculateProbabilities(List<Dice> diceList)
        {
            var probabilities = new Dictionary<(Dice, Dice), double>();

            foreach (var diceA in diceList)
            {
                foreach (var diceB in diceList)
                {
                    if (diceA == diceB) continue;

                    int wins = 0, total = 0;

                    foreach (var faceA in diceA.Faces)
                    {
                        foreach (var faceB in diceB.Faces)
                        {
                            if (faceA > faceB) wins++;
                            total++;
                        }
                    }

                    probabilities[(diceA, diceB)] = (double)wins / total;
                }
            }

            return probabilities;
        }

        public static string GetProbabilityTable(Dictionary<(Dice, Dice), double> probabilities)
        {
            var diceSet = probabilities.Keys.Select(k => k.Item1).Distinct().ToList();

            var headers = new List<string> { "Dice \\ vs >" };
            headers.AddRange(diceSet.Select(d => d.ToString()));

            var table = new ConsoleTable(headers.ToArray());

            foreach (var rowDice in diceSet)
            {
                var row = new List<string> { rowDice.ToString() };

                foreach (var colDice in diceSet)
                {
                    if (rowDice == colDice)
                        row.Add("--");
                    else
                        row.Add(probabilities[(rowDice, colDice)].ToString("P0"));
                }

                if (row.Count != headers.Count)
                {
                    throw new InvalidOperationException(
                        $"Row count mismatch: expected {headers.Count}, got {row.Count}.");
                }

                table.AddRow(row.ToArray());
            }

            return "Probability table (row beats column):\n" + table.ToMinimalString();
        }
    }
}