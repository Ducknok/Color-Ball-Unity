using UnityEngine;
using System.Collections.Generic; // Cần thư viện này để dùng List

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Upgrade/Card Data", fileName = "New Upgrade Card")]
public class UpgradeCardData : ScriptableObject
{
    [Header("Basic Info")]
    public Rarity rarity;
    public Sprite icon;
    public UpgradeType upgradeType;


    [System.Serializable] 
    public class LevelData
    {
        [TextArea(2, 5)]
        public string description; 
        public float value;        

    }

    [Header("Levels Configuration")]

    public List<LevelData> levels;


    public LevelData GetLevelData(int currentLevel)
    {

        int index = currentLevel - 1;


        if (levels == null || levels.Count == 0) return new LevelData { description = "No Data", value = 0 };


        if (index >= levels.Count) index = levels.Count - 1;

        if (index < 0) index = 0;

        return levels[index];
    }


    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        EditorApplication.delayCall += RenameAsset;
#endif
    }

#if UNITY_EDITOR
    private void RenameAsset()
    {
        if (this == null) return;

        string newName = $"{upgradeType}_{rarity}";
        string assetPath = AssetDatabase.GetAssetPath(this);

        if (!string.IsNullOrEmpty(assetPath) && name != newName)
        {
            AssetDatabase.RenameAsset(assetPath, newName);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}