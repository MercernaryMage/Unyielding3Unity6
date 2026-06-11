using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TileScriptableObjectCollection))]
public class TileScriptableObjectCollectionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		// Get reference to the target script
		TileScriptableObjectCollection tileCollection = (TileScriptableObjectCollection)target;

		if (GUILayout.Button("Update"))
		{
			FilloutData(tileCollection);
		}
	}

	public static void FilloutData(TileScriptableObjectCollection tileCollection)
	{
		tileCollection.tiles.Clear();

		string filter = "t:TileScriptableObject";

		string[] guids = AssetDatabase.FindAssets(filter);

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			tileCollection.tiles.Add(AssetDatabase.LoadAssetAtPath<TileScriptableObject>(path));
		}
		EditorUtility.SetDirty(tileCollection);
		AssetDatabase.SaveAssets();
	}
}
