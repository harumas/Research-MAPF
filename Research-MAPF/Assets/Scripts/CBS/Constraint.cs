namespace PathFinding.CBS
{
    public readonly struct Constraint
    {
        public readonly Node Node; // node position
        public readonly int Time; // time step

        public Constraint(Node node, int time)
        {
            this.Node = node;
            this.Time = time;
        }
    }
}