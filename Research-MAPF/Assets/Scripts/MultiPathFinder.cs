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

        private bool isInitialized;
        private bool isPathFound;
        private IFindStrategy pathFinder;

        private void Start()
        {
            isInitialized = mediator.Initialize();
            pathFinder = new CBS.CBS(mediator.ConstructGraph(), mediator);
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
            agentPathList = pathFinder.FindSolution(contexts);

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

        private void Clear()
        {
            agentPathList.Clear();
            isPathFound = false;
        }
    }
}