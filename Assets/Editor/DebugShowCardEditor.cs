using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugShowCard))]
public class DebugShowCardEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		DebugShowCard debugShowCard = (DebugShowCard)target;

		if (GUILayout.Button("Show Card"))
		{
			debugShowCard.ShowCard();
		}
	}
}
