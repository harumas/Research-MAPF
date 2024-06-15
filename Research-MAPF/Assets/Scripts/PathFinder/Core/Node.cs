using System;
using System.Numerics;

namespace PathFinder.Core
{
    public readonly struct Node
    {
        public readonly int Index;
        public readonly Vector2 Position;

        public Node(int index, Vector2 position)
        {
            Index = index;
            Position = position;
        }

        public static bool operator ==(Node a, Node b) => a.Index == b.Index;
        public static bool operator !=(Node a, Node b) => !(a == b);

        public bool Equals(Node other)
        {
            return Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            return obj is Node other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, Position);
        }
    }
}