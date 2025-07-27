namespace NonTransitiveDice
{
    public class Dice
    {
        public int[] Faces { get; }

        public Dice(int[] faces)
        {
            Faces = faces ?? throw new ArgumentNullException(nameof(faces));
            if (faces.Length == 0) throw new ArgumentException("Dice must have at least one face");
        }

        public int Roll(int faceIndex) => Faces[faceIndex];

        public override string ToString() => $"[{string.Join(",", Faces)}]";
    }
}