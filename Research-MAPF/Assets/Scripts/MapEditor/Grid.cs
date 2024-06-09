using System;
using UnityEngine;

namespace PathFinding
{
    public class Grid : MonoBehaviour
    {
        public event Action<Vector2Int> OnSelected;
        public event Action<Vector2Int> OnUnselected;
        public Vector2Int Point { get; private set; }

        private Renderer blockRenderer;

        private void Start()
        {
            blockRenderer = GetComponent<Renderer>();
            var position = transform.localPosition;
            Point = new Vector2Int((int)position.x, (int)position.z); 
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                OnSelected?.Invoke(Point);
            }

            if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
            {
                OnUnselected?.Invoke(Point);
            }

            //デバッグ用
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && Input.GetKey(KeyCode.LeftShift))
            {
                blockRenderer.material.color = Color.green;
            }

            if ((Input.GetMouseButtonDown(0)) && Input.GetKey(KeyCode.LeftControl))
            {
                Debug.Log($"This grid is {Point}");
            }
        }
    }
}