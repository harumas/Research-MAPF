using System.Collections.Generic;
using UnityEngine;

namespace Visualiser
{
    public class MoveAgent : MonoBehaviour
    {
        public int Index { get; private set; }

        private List<Vector2Int> gridPositions;
        [SerializeField] private float speed;
        private int moveCount;
        private Vector3 agentPosition;

        public void Initialize(int index, Vector2Int start, Color color)
        {
            Index = index;

            gameObject.name = $"Agent_{index}";
            transform.localPosition = GetAgentPos(start);
            GetComponent<Renderer>().material.color = color;
        }

        public void SetWaypoints(List<Vector2Int> gridPositions)
        {
            this.gridPositions = gridPositions;
            agentPosition = GetAgentPos(gridPositions[moveCount]);
        }

        private void Update()
        {
            //Enterキーを押したら進む
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (gridPositions == null)
                {
                    return;
                }
                
                moveCount++;

                //進みきったら終了
                if (moveCount >= gridPositions.Count)
                {
                    return;
                }

                agentPosition = GetAgentPos(gridPositions[moveCount]);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyImmediate(gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (gridPositions == null)
            {
                return;
            }

            if (gridPositions != null && moveCount >= gridPositions.Count)
            {
                return;
            }

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, agentPosition, Time.fixedDeltaTime * speed);
        }

        private Vector3 GetAgentPos(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, 1f, gridPos.y);
        }
    }
}