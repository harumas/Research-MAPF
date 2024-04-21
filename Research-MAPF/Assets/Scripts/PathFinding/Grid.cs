using System;
using UnityEngine;

namespace PathFinding
{
    public class Grid : MonoBehaviour
    {
        public event Action<Vector2Int> OnSelected;
        public event Action<Vector2Int> OnUnselected;

        private void OnMouseEnter()
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
        }
    }
}