using UnityEngine;
using UnityEditor;

public class ScriptableObjectMenu
{
    [MenuItem("Assets/Create/Items/Item Def")]
    static void NewItemDef()
    {
        var asset = ScriptableObject.CreateInstance<ItemDef>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);

        path += "/New ItemDef.asset";

        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
