using UnityEngine;

namespace PathFinding.CBS
{
    public class Node
    {
        public readonly int Index;
        public readonly Vector2 Position;
        public bool Available;
        public int Time;
        
        //ノード間の移動コストの合計
        public int VCost;
        
        //g(x) + h(x)の合計
        public float GCost;
        
        //ゴールまでのヒューリスティック
        public float HCost;
        public Node Parent;

        public Node(int index, Vector2 position)
        {
            Index = index;
            Position = position;

            Reset();
        }

        public void Reset()
        {
            Available = true;
            Time = 0;
            VCost = 0;
            GCost = 0;
            HCost = 0;
            Parent = null;
        }
    }
}