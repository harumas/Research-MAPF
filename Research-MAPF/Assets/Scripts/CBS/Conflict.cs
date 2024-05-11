﻿using System.Collections.Generic;

namespace PathFinding.CBS
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
    }
}