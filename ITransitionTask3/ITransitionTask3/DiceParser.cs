namespace NonTransitiveDice
{
    public class DiceParser
    {
        public static List<Dice> Parse(string[] input)
        {
            if (input == null || input.Length < 3)
                throw new ArgumentException("At least 3 dice must be provided. Example: 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3");

            var diceList = new List<Dice>();

            for (int i = 0; i < input.Length; i++)
            {
                string diceStr = input[i];

                if (string.IsNullOrWhiteSpace(diceStr))
                    throw new ArgumentException($"Dice #{i + 1} is empty.");

                var parts = diceStr.Split(',');

                if (parts.Length < 1)
                    throw new ArgumentException($"Dice #{i + 1} must have at least one face.");

                var faces = new List<int>();

                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int face))
                        throw new ArgumentException($"Dice #{i + 1} contains a non-integer value: '{part}'");

                    if (face <= 0)
                        throw new ArgumentException($"Dice #{i + 1} contains a non-positive side: {face}");

                    faces.Add(face);
                }

                if (faces.Count < 2)
                    throw new ArgumentException($"Dice #{i + 1} must have at least 2 sides.");

                diceList.Add(new Dice(faces.ToArray()));
            }

            return diceList;
        }

    }
}