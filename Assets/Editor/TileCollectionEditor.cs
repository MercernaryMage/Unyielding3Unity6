using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileCollection))]
public class TileCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Get reference to the target script
        TileCollection tileCollection = (TileCollection)target;

        if (GUILayout.Button("Update"))
        {
            FilloutData(tileCollection);
        }
    }

    public static void FilloutData(TileCollection tileCollection)
    {
        tileCollection.tiles.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                Tile tile = prefab.GetComponent<Tile>();

                if (tile != null)
                {
                    tileCollection.tiles.Add(tile);
                }
            }
        }

        EditorUtility.SetDirty(tileCollection);
        AssetDatabase.SaveAssets();
    }
}
