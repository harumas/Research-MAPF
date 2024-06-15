using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Visualiser.MapEditor
{
    [CreateAssetMenu(menuName = "MapData")]
    public class MapSaveData : ScriptableObject
    {
        [SerializeField] private List<EndPoint> endPoints;
        
        [Multiline(lines: 21)]
        [SerializeField]
        private string data;

        public string Data => data;
        public IReadOnlyList<EndPoint> EndPoints => endPoints;

        public void SetData(string data)
        {
            this.data = data;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}