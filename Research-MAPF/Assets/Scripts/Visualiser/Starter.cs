﻿using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;
using UnityEngine;
using Visualiser.MapEditor;


namespace Visualiser
{
    public enum FindStrategy
    {
        NormalBFS,
        CBS,
        CCBS
    }

    public class Starter : MonoBehaviour
    {
        [Header("探索アルゴリズム")]
        [SerializeField] private FindStrategy findStrategy;
        
        [Header("インスタンス")]
        [SerializeField] private GridGraphMediator mediator;
        [SerializeField] private GameObject agentPrefab;
        [SerializeField] private Transform agentParent;

        private bool isInitialized;
        private Dictionary<FindStrategy, ISolver> solvers;
        private List<MoveAgent> moveAgents;
        private List<(int, List<int>)> agentPathList;
        private List<SolveContext> solveContexts;

        private void Start()
        {
            isInitialized = mediator.Initialize();

            // グラフを構築する
            Graph graph = mediator.ConstructGraph();

            // 探索アルゴリズムの作成
            SolverFactory factory = new SolverFactory(graph, mediator);
            solvers = factory.CreateSolvers();

            // スタートとゴールをマークする
            mediator.PaintEndPoints();

            // 検索用の情報を作成
            var endPoints = mediator.GetEndPoints();
            solveContexts = CreateContexts(endPoints);
        }

        public void FindPath()
        {
            if (!isInitialized)
            {
                Debug.LogError("初期化されていないため、探索を開始できません。");
                return;
            }

            //探索する
            ISolver solver = solvers[findStrategy];
            agentPathList = solver.Solve(solveContexts);

            //nullが帰ってきたら解決不能とする
            if (agentPathList == null)
            {
                Debug.LogError("Can't Solve!");
                return;
            }

            //エージェントにパスを設定
            foreach ((int agentIndex, List<int> path) in agentPathList)
            {
                mediator.PaintPath(path);

                MoveAgent moveAgent = moveAgents[agentIndex];
                moveAgent.SetWaypoints(path.Select(node => mediator.GetPos(node)).ToList());
            }
        }

        /// <summary>
        /// Solverに渡すContextを作成します
        /// </summary>
        /// <param name="endPoints"></param>
        /// <returns></returns>
        private List<SolveContext> CreateContexts(IReadOnlyList<EndPoint> endPoints)
        {
            List<SolveContext> contexts = new List<SolveContext>();
            moveAgents = new List<MoveAgent>(endPoints.Count);

            int index = 0;
            foreach (EndPoint endPoint in endPoints)
            {
                //エージェントのオブジェクトを生成
                MoveAgent moveAgent = Instantiate(agentPrefab, agentParent).GetComponent<MoveAgent>();
                moveAgent.Initialize(index, endPoint.Start, mediator.Colors[index]);

                //座標をノードに変換
                int startNode = mediator.GetNode(endPoint.Start);
                int endNode = mediator.GetNode(endPoint.Goal);

                contexts.Add(new SolveContext(index, startNode, endNode));
                moveAgents.Add(moveAgent);
                index++;
            }

            return contexts;
        }
    }
}