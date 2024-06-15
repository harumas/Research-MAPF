using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
    public readonly struct Constraint
    {
        public readonly Node Node; 
        public readonly int Time; 

        public Constraint(Node node, int time)
        {
            this.Node = node;
            this.Time = time;
        }
    }
}