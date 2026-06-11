using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PropCollection))]
public class PropCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Get reference to the target script
        PropCollection propCollection = (PropCollection)target;

        if (GUILayout.Button("Update"))
        {
            FilloutData(propCollection);
        }
    }

    public static void FilloutData(PropCollection propCollection)
    {
        propCollection.props.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Props/Prefabs" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            propCollection.props.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }

        EditorUtility.SetDirty(propCollection);
        AssetDatabase.SaveAssets();
    }
}
