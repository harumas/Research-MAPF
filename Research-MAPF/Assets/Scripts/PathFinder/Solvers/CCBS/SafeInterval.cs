namespace PathFinder.Solvers.CCBS
{
    public readonly struct SafeInterval
    {
        public readonly float Start;
        public readonly float End;

        public SafeInterval(float start, float end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"[{Start}, {End}]";
        }
    }
}