using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathFinding
{
    [Serializable]
    public struct Agent
    {
        public Vector2Int Start;
        public Vector2Int Goal;
    }

    public class MultiPathFinder : MonoBehaviour
    {
        [SerializeField] private GridGraphMediator mediator;
        [SerializeField] private List<Agent> agents;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;
        [SerializeField] private Color pathColor;

        private PathFinder pathFinder;
        private bool isInitialized;

        private void Start()
        {
            mediator.Initialize();
            isInitialized = InitializePoints();
            pathFinder = new PathFinder(mediator);
        }

        private bool InitializePoints()
        {
            bool isUniqueStarts = !agents.GroupBy(p => p.Start).SelectMany(g => g.Skip(1)).Any();
            bool isUniqueGoals = !agents.GroupBy(p => p.Goal).SelectMany(g => g.Skip(1)).Any();
            bool isUniquePoints = isUniqueStarts && isUniqueGoals;

            if (isUniquePoints)
            {
                foreach (Agent agent in agents)
                {
                    //スタートとゴールに色を付ける
                    int startNode = mediator.GetNode(agent.Start);
                    int goalNode = mediator.GetNode(agent.Goal);

                    mediator.GetCell(startNode).SetColor(startColor);
                    mediator.GetCell(goalNode).SetColor(endColor);
                }
            }
            else
            {
                Debug.LogError("スタートとゴールのデータが重複しています。");
            }

            return isUniquePoints;
        }

        public void FindPath()
        {
            if (!isInitialized)
            {
                Debug.LogError("初期化されていないため、探索を開始できません。");
                return;
            }

            foreach (Agent agent in agents)
            {
                int startNode = mediator.GetNode(agent.Start);
                int endNode = mediator.GetNode(agent.Goal);

                //最短経路を探索
                List<int> shortestPath = pathFinder.FindPath(startNode, endNode);

                Debug.Log(shortestPath);

                //startとgoalを除いた部分を塗る
                for (var i = 1; i < shortestPath.Count - 1; i++)
                {
                    var node = shortestPath[i];
                    mediator.GetCell(node).SetColor(pathColor);
                }
            }
        }
    }
}