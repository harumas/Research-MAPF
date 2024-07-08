using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;
using UnityEditor;
using UnityEngine;
using Visualiser.MapEditor;
using Wanna.DebugEx;

namespace Visualiser
{
    /// <summary>
    /// シーン上のグリッドとグラフを仲介するクラス
    /// </summary>
    public class GridGraphMediator : MonoBehaviour
    {
        public List<Color> Colors => colors;

        [Header("エージェントの色")] [SerializeField] private List<Color> colors;
        [SerializeField] private Color pathColor;
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private bool showNodeIndex;

        private MapData mapData;
        private Dictionary<int, int> nodeIndexList;
        private Dictionary<int, int> indexNodeList;
        private Graph graph;

        private readonly Vector2Int[] direction = new[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        public bool Initialize()
        {
            mapData = mapGenerator.Generate();
            nodeIndexList = CreateNodeIndexList();
            indexNodeList = nodeIndexList.ToDictionary(x => x.Value, x => x.Key);
            return ValidateEndPoints();
        }

        public Graph ConstructGraph()
        {
            graph = new Graph(mapData.PassableCount);

            for (int y = 0; y < mapData.Height; y++)
            {
                for (int x = 0; x < mapData.Width; x++)
                {
                    int fromIndex = y * mapData.Width + x;
                    NodeGrid from = mapData.Grids[fromIndex];

                    //障害物のバスならNodeを作らない
                    if (!from.IsPassable)
                    {
                        continue;
                    }

                    //４方向にEdgeをつなぐ
                    foreach (Vector2Int dir in direction)
                    {
                        Vector2Int pos = new Vector2Int(x, y) + dir;
                        bool isConnectable = 0 <= pos.x && pos.x < mapData.Width && 0 <= pos.y && pos.y < mapData.Height;

                        if (isConnectable)
                        {
                            int toIndex = pos.y * mapData.Width + pos.x;
                            NodeGrid to = mapData.Grids[toIndex];

                            if (to.IsPassable)
                            {
                                graph.AddEdge(GetNode(fromIndex), GetNode(toIndex));
                            }
                        }
                    }
                }
            }

            return graph;
        }

        public IReadOnlyList<EndPoint> GetEndPoints()
        {
            return mapGenerator.GetMapSaveData().EndPoints;
        }

        public void PaintEndPoints()
        {
            IReadOnlyList<EndPoint> endPoints = GetEndPoints();

            for (var i = 0; i < endPoints.Count; i++)
            {
                EndPoint endPoint = endPoints[i];
                //スタートとゴールに色付け
                GetGrid(GetNode(endPoint.Start)).ActivateEndPoint(colors[i]);
                GetGrid(GetNode(endPoint.Goal)).ActivateEndPoint(colors[i]);
            }
        }

        public void PaintPath(List<int> path)
        {
            IReadOnlyList<EndPoint> endPoints = GetEndPoints();
            int[] endPointNodes = endPoints.Select(point => GetNode(point.Start))
                .Concat(endPoints.Select(point => GetNode(point.Goal)))
                .ToArray();

            //パスに色付け
            for (var i = 1; i < path.Count - 1; i++)
            {
                var node = path[i];

                if (!endPointNodes.Contains(node))
                {
                    NodeGrid grid = GetGrid(node);
                    grid.SetColor(pathColor);
                }
            }
        }

        public int GetNode(Vector2Int pos)
        {
            int index = pos.y * mapData.Width + pos.x;
            return indexNodeList[index];
        }

        public Vector2Int GetPos(int node)
        {
            int index = nodeIndexList[node];
            return new Vector2Int(index % mapData.Width, index / mapData.Width);
        }

        private int GetNode(int index)
        {
            return indexNodeList[index];
        }

        private int GetIndex(int node)
        {
            return nodeIndexList[node];
        }

        private NodeGrid GetGrid(int node)
        {
            return mapData.Grids[GetIndex(node)];
        }

        private bool ValidateEndPoints()
        {
            var endPoints = mapGenerator.GetMapSaveData().EndPoints;
            bool isUniqueStarts = !endPoints.GroupBy(p => p.Start).SelectMany(g => g.Skip(1)).Any();
            bool isUniqueGoals = !endPoints.GroupBy(p => p.Goal).SelectMany(g => g.Skip(1)).Any();
            bool isUniquePoints = isUniqueStarts && isUniqueGoals;

            if (!isUniquePoints)
            {
                Debug.LogError("スタートとゴールのデータが重複しています。");
            }

            return isUniquePoints;
        }

        private Dictionary<int, int> CreateNodeIndexList()
        {
            Dictionary<int, int> list = new Dictionary<int, int>();

            int nodeCount = 0;
            for (int y = 0; y < mapData.Height; y++)
            {
                for (int x = 0; x < mapData.Width; x++)
                {
                    int index = y * mapData.Width + x;
                    NodeGrid grid = mapData.Grids[index];

                    if (grid.IsPassable)
                    {
                        list.Add(nodeCount++, index);
                    }
                }
            }

            return list;
        }

        private void OnDrawGizmos()
        {
            if (showNodeIndex)
            {
                foreach (int node in nodeIndexList.Keys)
                {
                    Vector2Int pos = GetPos(node);
                    Vector3 offset = new Vector3(-0.4f, 0f, 1f);
                    Handles.Label(new Vector3(pos.x, 0f, pos.y) + mapGenerator.transform.position + offset, node.ToString().Color(Color.red),
                        new GUIStyle() { richText = true });
                }
            }
        }
    }
}