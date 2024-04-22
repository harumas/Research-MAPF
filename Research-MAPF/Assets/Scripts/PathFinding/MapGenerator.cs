using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PathFinding
{
    public readonly struct Cell
    {
        public readonly bool IsPassable;
        public readonly Renderer Renderer;
        public readonly Color DefaultColor;

        public Cell(bool isPassable, Renderer renderer, Color defaultColor, Color color)
        {
            IsPassable = isPassable;
            Renderer = renderer;
            DefaultColor = defaultColor;
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            Renderer.material.color = color;
        }

        public void ResetColor()
        {
            Renderer.material.color = DefaultColor;
        }
    }

    public class MapData
    {
        public readonly int Height;
        public readonly int Width;
        public readonly int PassableCount;
        public readonly List<Cell> Cells;

        public MapData(int height, int width, int passableCount, List<Cell> cells)
        {
            Height = height;
            Width = width;
            PassableCount = passableCount;
            Cells = cells;
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private int defaultHeight;
        [SerializeField] private int defaultWidth;
        [SerializeField] private MapSaveData mapData;

        private int[,] mapIdData;
        private MapData currentMapData;
        private bool isEditMode;

        [SerializeField] private GameObject prefab;
        [SerializeField] private Color firstColor;
        [SerializeField] private Color secondColor;
        [SerializeField] private Color obstacleColor;

        [SerializeField] private TextMeshProUGUI obstacleModeText;

        public void SwitchEditMode()
        {
            isEditMode = !isEditMode;

            if (isEditMode)
            {
                mapIdData = ParseMapData();
                obstacleModeText.SetText("Export");
            }
            else
            {
                char[] str = new char[defaultWidth * defaultHeight + defaultWidth];
                Array.Fill(str, '.');

                for (int i = 0; i < mapIdData.GetLength(0); i++)
                {
                    for (int j = 0; j < mapIdData.GetLength(1); j++)
                    {
                        int v = mapIdData[i, j];
                        str[i * defaultWidth + j + i] = v == 0 ? '.' : '*';
                    }

                    str[i * defaultWidth + defaultHeight + i] = '\n';
                }

                string result = new string(str);
                mapData.SetData(result);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                obstacleModeText.SetText("Edit");
            }
        }

        private int[,] ParseMapData()
        {
            string[] data = mapData.Data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int width = data.Length != 0 ? data[0].Length : defaultWidth;
            int height = data.Length != 0 ? data.Length : defaultHeight;

            int[,] mapIds = new int[height, width];

            for (var i = 0; i < data.Length; i++)
            {
                var h = data[i];

                for (var j = 0; j < h.Length; j++)
                {
                    mapIds[i, j] = h[j] == '.' ? 0 : 1;
                }
            }

            return mapIds;
        }

        public MapData Generate()
        {
            int[,] mapIds;
            int width, height;

            if (mapData.Data.Length == 0)
            {
                mapIds = new int[defaultHeight, defaultWidth];
                width = defaultWidth;
                height = defaultHeight;
            }
            else
            {
                mapIds = ParseMapData();
                height = mapIds.GetLength(0);
                width = mapIds.GetLength(1);
            }

            transform.position = Vector3.zero;
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            List<Cell> cells = new List<Cell>();
            bool first = true;
            int passableCount = 0;

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = new Vector3(x, 0f, z);
                    bool isPassable = mapIds[z, x] == 0;

                    GameObject cube = Instantiate(prefab, transform);
                    Renderer renderer = cube.GetComponent<Renderer>();
                    cube.transform.position = position;

                    Grid grid = cube.GetComponent<Grid>();
                    grid.OnSelected += SetObstacle;
                    grid.OnUnselected += UnsetObstacle;

                    Color defaultColor = (x + z) % 2 != 0 ? firstColor : secondColor;
                    Color color = isPassable ? defaultColor : obstacleColor;
                    Cell cell = new Cell(isPassable, renderer, defaultColor, color);

                    cells.Add(cell);

                    if (isPassable)
                    {
                        passableCount++;
                    }

                    first = !first;
                }
            }

            transform.position = new Vector3(-height * 0.5f + 0.5f, 0f, -width * 0.5f + 0.5f);

            currentMapData = new MapData(height, width, passableCount, cells);

            return currentMapData;
        }

        public void SetObstacle(Vector2Int point)
        {
            if (!isEditMode)
            {
                return;
            }

            mapIdData[point.y, point.x] = 1;

            Cell cell = currentMapData.Cells[point.y * defaultWidth + point.x];
            cell.SetColor(obstacleColor);
        }

        private void UnsetObstacle(Vector2Int point)
        {
            if (!isEditMode)
            {
                return;
            }

            mapIdData[point.y, point.x] = 0;

            Cell cell = currentMapData.Cells[point.y * defaultWidth + point.x];
            cell.ResetColor();
        }
    }
}