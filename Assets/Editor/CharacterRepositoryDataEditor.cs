using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(CharacterRepositoryData))]
public class CharacterRepositoryDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		// Get reference to the target script
		CharacterRepositoryData data = (CharacterRepositoryData)target;

		// Add a button
		if (GUILayout.Button("Update"))
		{
			FilloutData(data);
		}
	}

	public static void FilloutData(CharacterRepositoryData data)
	{
		data.characters.Clear();

		string filter = "t:CharacterScriptableObject";

		string[] guids = AssetDatabase.FindAssets(filter);

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			data.characters.Add(AssetDatabase.LoadAssetAtPath<CharacterScriptableObject>(path));
		}
		EditorUtility.SetDirty(data);
		AssetDatabase.SaveAssets();
	}
}
