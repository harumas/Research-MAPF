using System.Collections.Generic;
using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
    public readonly struct Constraint
    {
        public readonly int AgentId;
        public readonly Node Node;
        public readonly int Time;

        public Constraint(int agentId, Node node, int time)
        {
            this.AgentId = agentId;
            this.Node = node;
            this.Time = time;
        }
        
        public static bool IsConstrained(int agentId, int node, IEnumerable<Constraint> constraints)
        {
            foreach (Constraint constraint in constraints)
            {
                if (agentId == constraint.AgentId)
                {
                    //その場に居なくてはならない
                    if (node != constraint.Node.Index)
                    {
                        return false;
                    }
                }
                else
                {
                    //その場に居てはならない
                    if (node == constraint.Node.Index)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}