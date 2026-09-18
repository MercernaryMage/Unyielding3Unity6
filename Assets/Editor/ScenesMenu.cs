using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesMenu
{
	static bool CanSwapScene()
	{
		Scene s = SceneManager.GetActiveScene();
		if (s.isDirty && s.name != "")
		{
			return false;
		}
		return true;
	}

	[MenuItem("Scenes/Combat")]
	private static void GoToCombat()
	{
		if (!CanSwapScene())
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Combat.unity");
	}

	[MenuItem("Scenes/Town")]
	private static void GoToTown()
	{
		if (!CanSwapScene())
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Town.unity");
	}

	[MenuItem("Scenes/Overland")]
	private static void GoToOverland()
	{
		if (!CanSwapScene())
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Overland.unity");
	}

	[MenuItem("Scenes/Tutorial Start")]
	private static void GoToTutorialStart()
	{
		if (!CanSwapScene())
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/TutorialStart.unity");
	}

	[MenuItem("Scenes/Level Editor")]
	private static void GoToLevelEditor()
	{
		if (!CanSwapScene())
		{
			Debug.LogWarning("Scene is dirty, please save");
			return;
		}
		EditorSceneManager.OpenScene("Assets/Scenes/Level Editor.unity");
	}
}
