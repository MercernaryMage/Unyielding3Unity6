using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugKillCharacter))]
public class DebugKillCharacterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		// Get reference to the target script
		DebugKillCharacter cardDisplay = (DebugKillCharacter)target;

		// Add a button
		if (GUILayout.Button("Kill Selected Character"))
		{
			cardDisplay.KillCharacter();
		}
	}
}
