using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatusEffectIconCollection))]
public class StatusEffectIconCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		// Get reference to the target script
		StatusEffectIconCollection statusEffectIconCollection = (StatusEffectIconCollection)target;

        if (GUILayout.Button("Update"))
        {
			FilloutData(statusEffectIconCollection);
        }
    }

	public static void FilloutData(StatusEffectIconCollection statusEffectIconCollection)
	{
		statusEffectIconCollection.icons.Clear();

		string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Status Effects/Icons" });

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			statusEffectIconCollection.icons.Add(AssetDatabase.LoadAssetAtPath<Sprite>(path));
		}

		EditorUtility.SetDirty(statusEffectIconCollection);
		AssetDatabase.SaveAssets();
	}
}
