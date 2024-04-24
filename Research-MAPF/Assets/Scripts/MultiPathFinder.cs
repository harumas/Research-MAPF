using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PathFinding
{
    public readonly struct SearchContext
    {
        public readonly Agent Agent;
        public readonly int Start;
        public readonly int Goal;

        public SearchContext(Agent agent, int start, int goal)
        {
            Agent = agent;
            Start = start;
            Goal = goal;
        }
    }

    [Serializable]
    public struct EndPoint
    {
        public Vector2Int Start;
        public Vector2Int Goal;
    }

    public class MultiPathFinder : MonoBehaviour
    {
        [SerializeField] private GridGraphMediator mediator;
        [SerializeField] private GameObject agentPrefab;
        [SerializeField] private Transform agentParent;

        private PathFinder pathFinder;
        private bool isInitialized;
        private bool isPathFound;

        private void Start()
        {
            isInitialized = mediator.Initialize();
            pathFinder = new PathFinder(mediator);
        }

        private List<(Agent, List<int>)> agentPathList;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Clear();
            }
        }

        public void FindPath()
        {
            if (!isInitialized)
            {
                Debug.LogError("初期化されていないため、探索を開始できません。");
                return;
            }

            if (isPathFound)
            {
                return;
            }

            var endPoints = mediator.GetEndPoints();

            //検索用の情報を生成
            var contexts = CreateContexts(endPoints);

            //探索する
            agentPathList = FindMultiAgentPath(contexts);

            //エージェントにパスを設定
            foreach ((Agent agent, List<int> path) in agentPathList)
            {
                mediator.PaintPath(path);
                agent.SetWaypoints(path.Select(node => mediator.GetPos(node)).ToList());
            }

            isPathFound = true;
        }

        private List<SearchContext> CreateContexts(IReadOnlyList<EndPoint> endPoints)
        {
            List<SearchContext> contexts = new List<SearchContext>();

            int index = 0;
            foreach (EndPoint endPoint in endPoints)
            {
                //エージェントのオブジェクトを生成
                Agent agent = Instantiate(agentPrefab, agentParent).GetComponent<Agent>();
                agent.Initialize(index++, endPoint.Start);

                //座標をノードに変換
                int startNode = mediator.GetNode(endPoint.Start);
                int endNode = mediator.GetNode(endPoint.Goal);

                contexts.Add(new SearchContext(agent, startNode, endNode));
            }

            return contexts;
        }

        private List<(Agent agent, List<int> path)> FindMultiAgentPath(List<SearchContext> contexts)
        {
            var pathList = new List<(Agent agent, List<int> path)>();

            foreach (SearchContext context in contexts)
            {
                List<int> shortestPath = pathFinder.FindPath(context.Start, context.Goal);
                pathList.Add((context.Agent, shortestPath));
            }

            //距離が近い順にソート
            List<(Agent agent, List<int> path)> orderedList = pathList.OrderBy(item => item.path.Count).ToList();

            //最大距離を取得
            int maxMove = orderedList.Select(item => item.path.Count).Max();

            for (int t = 0; t < maxMove; t++)
            {
                for (var i = 1; i < orderedList.Count; i++)
                {
                    List<int> current = orderedList[i].path;

                    if (t >= current.Count)
                    {
                        continue;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        List<int> target = orderedList[j].path;
                        if (t >= target.Count)
                        {
                            continue;
                        }

                        //エージェントが衝突したらタイミングをずらす
                        if (current[t] == target[t])
                        {
                            current.Insert(t, current[t - 1]);
                        }
                    }
                }
            }

            return orderedList;
        }

        private void Clear()
        {
            agentPathList.Clear();
            isPathFound = false;
        }
    }
}