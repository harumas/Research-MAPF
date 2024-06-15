using System;
using UnityEngine;

namespace Visualiser.MapEditor
{
    [Serializable]
    public struct EndPoint
    {
        public Vector2Int Start;
        public Vector2Int Goal;
    }
}