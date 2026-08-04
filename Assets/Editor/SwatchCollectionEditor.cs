using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwatchCollection))]
public class SwatchCollectionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SwatchCollection swatchCollection = (SwatchCollection)target;

		if (GUILayout.Button("Update"))
		{
			FilloutData(swatchCollection);
		}
	}

	public static void FilloutData(SwatchCollection swatchCollection)
	{
		swatchCollection.swatches.Clear();

		string filter = "t:SwatchScriptableObject";

		string[] guids = AssetDatabase.FindAssets(filter);

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			swatchCollection.swatches.Add(AssetDatabase.LoadAssetAtPath<SwatchScriptableObject>(path));
		}
		EditorUtility.SetDirty(swatchCollection);
		AssetDatabase.SaveAssets();
	}
}
