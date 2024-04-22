using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathFinding
{
    public class GridGraphMediator : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;

        private MapData mapData;
        private Dictionary<int, int> nodeIndexList;
        private Dictionary<int, int> indexNodeList;

        private readonly Vector2Int[] direction = new[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        public void Initialize()
        {
            mapData = mapGenerator.Generate();
            nodeIndexList = CreateNodeIndexList();
            indexNodeList = nodeIndexList.ToDictionary(x => x.Value, x => x.Key);
        }

        public int GetNode(int index)
        {
            return indexNodeList[index];
        }

        public int GetNode(Vector2Int pos)
        {
            int index = pos.y * mapData.Width + pos.x;
            return indexNodeList[index];
        }

        public int GetIndex(int node)
        {
            return nodeIndexList[node];
        }

        public Vector2Int GetPos(int node)
        {
            int index = nodeIndexList[node];
            return new Vector2Int(index % mapData.Width, index / mapData.Width);
        }

        public Cell GetCell(int node)
        {
            return mapData.Cells[GetIndex(node)];
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
                    Cell cell = mapData.Cells[index];

                    if (cell.IsPassable)
                    {
                        int node = nodeCount++;
                        list.Add(node, index);
                    }
                }
            }

            return list;
        }

        public Graph ConstructGraph()
        {
            Graph graph = new Graph(mapData.PassableCount);

            for (int y = 0; y < mapData.Height; y++)
            {
                for (int x = 0; x < mapData.Width; x++)
                {
                    int fromIndex = y * mapData.Width + x;
                    Cell from = mapData.Cells[fromIndex];

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
                            Cell to = mapData.Cells[toIndex];

                            if (to.IsPassable)
                            {
                                graph.AddEdge(indexNodeList[fromIndex], indexNodeList[toIndex]);
                            }
                        }
                    }
                }
            }

            return graph;
        }
    }
}