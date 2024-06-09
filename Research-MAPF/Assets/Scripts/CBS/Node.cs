using UnityEngine;

namespace PathFinding.CBS
{
    public class Node
    {
        public readonly int Index;
        public readonly Vector2 Position;
        
        public Node(int index, Vector2 position)
        {
            Index = index;
            Position = position;
        }
    }
}