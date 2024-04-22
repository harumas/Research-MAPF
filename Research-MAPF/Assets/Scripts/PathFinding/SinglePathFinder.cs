using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class SinglePathFinder : MonoBehaviour
    {
        [SerializeField] private GridGraphMediator mediator;
        [SerializeField] private Vector2Int start;
        [SerializeField] private Vector2Int goal;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;
        [SerializeField] private Color pathColor;

        private PathFinder pathFinder;

        private void Start()
        {
            mediator.Initialize();

            int startNode = mediator.GetNode(start);
            int endNode = mediator.GetNode(goal);

            //スタートとゴールに色を付ける
            mediator.GetCell(startNode).SetColor(startColor);
            mediator.GetCell(endNode).SetColor(endColor);

            pathFinder = new PathFinder(mediator);
        }

        public void FindPath()
        {
            int startNode = mediator.GetNode(start);
            int endNode = mediator.GetNode(goal);

            //最短経路を探索
            List<int> shortestPath = pathFinder.FindPath(startNode, endNode);

            //startとgoalを除いた部分を塗る
            for (var i = 1; i < shortestPath.Count - 1; i++)
            {
                var node = shortestPath[i];
                mediator.GetCell(node).SetColor(pathColor);
            }
        }
    }
}