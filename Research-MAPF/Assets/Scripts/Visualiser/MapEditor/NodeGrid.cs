using System;
using UnityEngine;

namespace Visualiser.MapEditor
{
    public class NodeGrid : MonoBehaviour
    {
        public Vector2 Position => transform.localPosition;
        public bool IsPassable { get; private set; }
        public Color DefaultColor { get; private set; }

        public event Action<Vector2Int> OnSelected;
        public event Action<Vector2Int> OnUnselected;

        [SerializeField] private Renderer blockRenderer;
        [SerializeField] private Renderer endPointRenderer;

        public void Initialize(bool isPassable, Color defaultColor, Color color)
        {
            IsPassable = isPassable;
            DefaultColor = defaultColor;
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            blockRenderer.material.color = color;
        }

        public void ResetColor()
        {
            blockRenderer.material.color = DefaultColor;
        }

        public void ActivateEndPoint(Color color)
        {
            endPointRenderer.material.color = color;
            endPointRenderer.enabled = true;
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                OnSelected?.Invoke(new Vector2Int((int)Position.x, (int)Position.y));
            }

            if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
            {
                OnUnselected?.Invoke(new Vector2Int((int)Position.x, (int)Position.y));
            }

            //デバッグ用
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && Input.GetKey(KeyCode.LeftShift))
            {
                blockRenderer.material.color = Color.green;
            }

            if ((Input.GetMouseButtonDown(0)) && Input.GetKey(KeyCode.LeftControl))
            {
                Debug.Log($"This grid is {Position}");
            }
        }
    }
}