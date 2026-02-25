using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Skin Data", fileName = "New Skin")]
public class SkinData : ScriptableObject
{
    public bool isOwned;
    public Rarity rarity;
    public string skinName;
    public Material material;
    public int price;

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

        string newName = $"{rarity}_{skinName}";
        string assetPath = AssetDatabase.GetAssetPath(this);

        if (!string.IsNullOrEmpty(assetPath) && name != newName)
        {
            AssetDatabase.RenameAsset(assetPath, newName);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}

