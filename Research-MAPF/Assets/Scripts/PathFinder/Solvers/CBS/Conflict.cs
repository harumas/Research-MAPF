using System.Collections.Generic;
using System.Text;
using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
    public class Conflict
    {
        public Node Node;
        public int Time = 0;
        public List<int> Agents;

        public Conflict(List<int> agents, Node node, int time)
        {
            this.Node = node;
            this.Agents = new List<int>(agents);
            this.Time = time;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (int agent in Agents)
            {
                builder.Append($"{{{agent}, {Node.Index}, {Time}}}\n");
            }

            return builder.ToString();
        }
    }
}