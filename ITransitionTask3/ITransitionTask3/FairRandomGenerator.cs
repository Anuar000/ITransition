using System.Security.Cryptography;

namespace NonTransitiveDice
{
    public class FairRandomGenerator
    {
        private readonly byte[] _key;
        private readonly int _computerChoice;
        private readonly HMACSHA256 _hmac;

        public FairRandomGenerator(int maxValue)
        {
            _key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_key);
            }

            _hmac = new HMACSHA256(_key);

            _computerChoice = GetSecureRandomInt(0, maxValue);
        }

        public string Hmac => BitConverter.ToString(_hmac.ComputeHash(
            BitConverter.GetBytes(_computerChoice))).Replace("-", "");

        public string Key => BitConverter.ToString(_key).Replace("-", "");

        public int ComputerChoice => _computerChoice;

        public int GetFairResult(int userChoice, int modulus) =>
            (userChoice + _computerChoice) % modulus;

        private int GetSecureRandomInt(int minValue, int maxValue)
        {
            if (minValue > maxValue) throw new ArgumentException();
            if (minValue == maxValue) return minValue;

            using (var rng = RandomNumberGenerator.Create())
            {
                uint range = (uint)(maxValue - minValue);
                byte[] randomNumber = new byte[4];
                uint randomValue;

                do
                {
                    rng.GetBytes(randomNumber);
                    randomValue = BitConverter.ToUInt32(randomNumber, 0);
                } while (randomValue > uint.MaxValue - (uint.MaxValue % range));

                return (int)(minValue + (randomValue % range));
            }
        }
    }
}