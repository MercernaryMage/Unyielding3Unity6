using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ItemCollection))]
public class ItemCollectionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ItemCollection itemCollection = (ItemCollection)target;

		if (GUILayout.Button("Update"))
		{
			FilloutData(itemCollection);
		}
	}

	public static void FilloutData(ItemCollection itemCollection)
	{
		itemCollection.items.Clear();

		string filter = "t:ItemScriptableObject";

		string[] guids = AssetDatabase.FindAssets(filter);

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			itemCollection.items.Add(AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(path));
		}
		EditorUtility.SetDirty(itemCollection);
		AssetDatabase.SaveAssets();
	}
}
