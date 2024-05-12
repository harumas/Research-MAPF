using System;
using UnityEngine;

namespace PathFinding
{
    public class Grid : MonoBehaviour
    {
        public event Action<Vector2Int> OnSelected;
        public event Action<Vector2Int> OnUnselected;

        private Renderer renderer;

        private void Start()
        {
            renderer = GetComponent<Renderer>();
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                var position = transform.localPosition;
                OnSelected?.Invoke(new Vector2Int((int)position.x, (int)position.z));
            }

            if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
            {
                var position = transform.localPosition;
                OnUnselected?.Invoke(new Vector2Int((int)position.x, (int)position.z));
            }

            //デバッグ用
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && Input.GetKey(KeyCode.LeftShift))
            {
                renderer.material.color = Color.green;
            }

            if ((Input.GetMouseButtonDown(0)) && Input.GetKey(KeyCode.LeftControl))
            {
                var position = transform.localPosition;
                var intPos = new Vector2Int((int)position.x, (int)position.z);
                Debug.Log($"This grid is {intPos}");
            }
        }
    }
}