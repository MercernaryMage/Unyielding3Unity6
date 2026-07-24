using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ExplanationItemCollection))]
public class ExplanationItemCollectionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ExplanationItemCollection explanationItemCollection = (ExplanationItemCollection)target;

		if (GUILayout.Button("Update"))
		{
			FilloutData(explanationItemCollection);
		}
	}

	public static void FilloutData(ExplanationItemCollection explanationItemCollection)
	{
		explanationItemCollection.explanationItems.Clear();

		string filter = "t:ExplanationIemScrptableObject";

		string[] guids = AssetDatabase.FindAssets(filter);

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			explanationItemCollection.explanationItems.Add(AssetDatabase.LoadAssetAtPath<ExplanationIemScrptableObject>(path));
		}
		EditorUtility.SetDirty(explanationItemCollection);
		AssetDatabase.SaveAssets();
	}
}
