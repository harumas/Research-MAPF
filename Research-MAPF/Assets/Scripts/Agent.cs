using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PathFinding
{
    public class Agent : MonoBehaviour
    {
        private List<Vector2Int> gridPositions;
        [SerializeField] private List<Color> colors;
        private int moveCount;

        public Color Color { get; private set; }

        public void Initialize(int index, Vector2Int start)
        {
            Color = colors.Count > index ? colors[index] : Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            transform.localPosition = GetAgentPos(start);
            GetComponent<Renderer>().material.color = Color;
        }

        public void SetWaypoints(List<Vector2Int> gridPositions)
        {
            this.gridPositions = gridPositions;
        }

        private void Update()
        {
            //Enterキーを押したら進む
            if (Input.GetKeyDown(KeyCode.Return))
            {
                moveCount++;

                //進みきったら終了
                if (moveCount >= gridPositions.Count)
                {
                    return;
                }

                Vector3 pos = GetAgentPos(gridPositions[moveCount]);
                transform.localPosition = pos;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyImmediate(gameObject);
            }
        }

        private Vector3 GetAgentPos(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, 1f, gridPos.y);
        }
    }
}