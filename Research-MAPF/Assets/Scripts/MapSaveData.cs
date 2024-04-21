using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "MapData")]
    public class MapSaveData : ScriptableObject
    {
        [Multiline]
        [SerializeField]
        private string data;

        public string Data => data;

        public void SetData(string data)
        {
            this.data = data;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}