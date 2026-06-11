using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesMenu
{
	[MenuItem("Scenes/Combat")]
	private static void GoToCombat()
	{
		if (SceneManager.GetActiveScene().isDirty)
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Combat.unity");
	}

	[MenuItem("Scenes/Town")]
	private static void GoToTown()
	{
		if (SceneManager.GetActiveScene().isDirty)
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Town.unity");
	}

	[MenuItem("Scenes/Overland")]
	private static void GoToOverland()
	{
		if (SceneManager.GetActiveScene().isDirty)
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Overland.unity");
	}

	[MenuItem("Scenes/Tutorial Start")]
	private static void GoToTutorialStart()
	{
		if (SceneManager.GetActiveScene().isDirty)
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/TutorialStart.unity");
	}
}
