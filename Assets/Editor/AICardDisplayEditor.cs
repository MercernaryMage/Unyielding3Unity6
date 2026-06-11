using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(AICardDisplay))]
public class AICardDisplayEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		// Get reference to the target script
		AICardDisplay cardDisplay = (AICardDisplay)target;

		// Add a button
		if (GUILayout.Button("Show Card"))
		{
			cardDisplay.ShowCardDebug();
		}
	}
}
