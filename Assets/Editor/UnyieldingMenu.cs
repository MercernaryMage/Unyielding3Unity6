using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnyieldingMenu
{
	[MenuItem("Unyielding/Disable All Debug In Combat")]
	static void DisableAllDebugInCombat()
	{
		string scenePath = "Assets/Scenes/Combat.unity";

		Type[] exemptTypes = new Type[] { typeof(DebugShowText), typeof(DebugKeyboardCommands) };
		HashSet<string> exemptGuids = new HashSet<string>();
		foreach (Type exemptType in exemptTypes)
		{
			string guid = FindScriptGuid(exemptType);
			if (guid == null)
			{
				Debug.LogError($"Could not find the script GUID for {exemptType.Name}");
				return;
			}
			exemptGuids.Add(guid);
		}

		string text = File.ReadAllText(scenePath);
		string newline = text.Contains("\r\n") ? "\r\n" : "\n";
		string[] lines = text.Split(new string[] { newline }, StringSplitOptions.None);

		// Find the GameObject document named Debug Code and collect its component fileIDs.
		int nameLineIndex = -1;
		for (int i = 0; i < lines.Length; ++i)
		{
			if (lines[i] == "  m_Name: Debug Code")
			{
				nameLineIndex = i;
				break;
			}
		}

		if (nameLineIndex == -1)
		{
			Debug.LogError($"Could not find a GameObject called Debug Code in {scenePath}");
			return;
		}

		int docStart = nameLineIndex;
		while (docStart >= 0 && !lines[docStart].StartsWith("--- !u!1 "))
		{
			--docStart;
		}

		HashSet<string> componentIds = new HashSet<string>();
		for (int i = docStart; i < nameLineIndex; ++i)
		{
			string trimmed = lines[i].Trim();
			if (trimmed.StartsWith("- component: {fileID: "))
			{
				int start = trimmed.IndexOf("fileID: ") + "fileID: ".Length;
				int end = trimmed.IndexOf('}', start);
				componentIds.Add(trimmed.Substring(start, end - start).Trim());
			}
		}

		// Disable every MonoBehaviour on Debug Code that isn't an exempt debug script.
		int disabledCount = 0;
		for (int i = 0; i < lines.Length; ++i)
		{
			if (!lines[i].StartsWith("--- !u!114 &"))
			{
				continue;
			}

			string fileId = lines[i].Substring("--- !u!114 &".Length).Trim();
			if (!componentIds.Contains(fileId))
			{
				continue;
			}

			int enabledLineIndex = -1;
			bool isExempt = false;
			for (int j = i + 1; j < lines.Length && !lines[j].StartsWith("--- "); ++j)
			{
				if (lines[j].StartsWith("  m_Enabled:"))
				{
					enabledLineIndex = j;
				}
				else if (lines[j].StartsWith("  m_Script:"))
				{
					foreach (string exemptGuid in exemptGuids)
					{
						if (lines[j].Contains(exemptGuid))
						{
							isExempt = true;
							break;
						}
					}
				}
			}

			if (!isExempt && enabledLineIndex != -1 && lines[enabledLineIndex] != "  m_Enabled: 0")
			{
				lines[enabledLineIndex] = "  m_Enabled: 0";
				++disabledCount;
			}
		}

		if (disabledCount > 0)
		{
			File.WriteAllText(scenePath, string.Join(newline, lines));
			AssetDatabase.ImportAsset(scenePath);
		}

		Scene openScene = EditorSceneManager.GetSceneByPath(scenePath);
		if (openScene.isLoaded)
		{
			Debug.LogWarning("Combat.unity is currently open in the editor; reload it (or discard and reopen) to pick up the changes made on disk.");
		}

		Debug.Log($"Disable All Debug In Combat: disabled {disabledCount} components on Debug Code");
	}

	static string FindScriptGuid(Type type)
	{
		foreach (string guid in AssetDatabase.FindAssets($"{type.Name} t:MonoScript"))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
			if (script != null && script.GetClass() == type)
			{
				return guid;
			}
		}
		return null;
	}

	[MenuItem("Unyielding/Fill out all Content")]
	static void FillOutAllContent()
	{
		GameObject characterObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/CharacterRepositoryData.prefab");
		CharacterRepositoryData characterData = characterObj.GetComponent<CharacterRepositoryData>();
		CharacterRepositoryDataEditor.FilloutData(characterData);

		GameObject itemObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/ItemCollection.prefab");
		ItemCollection itemCollection = itemObj.GetComponent<ItemCollection>();
		ItemCollectionEditor.FilloutData(itemCollection);

		GameObject propObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/PropCollection.prefab");
		PropCollection propCollection = propObj.GetComponent<PropCollection>();
		PropCollectionEditor.FilloutData(propCollection);

		GameObject statusEffectIconObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/StatusEffectIconCollection.prefab");
		StatusEffectIconCollection statusEffectIconCollection = statusEffectIconObj.GetComponent<StatusEffectIconCollection>();
		StatusEffectIconCollectionEditor.FilloutData(statusEffectIconCollection);

		GameObject tileCollectionObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/TileCollection.prefab");
		TileCollection tileCollection = tileCollectionObj.GetComponent<TileCollection>();
		TileCollectionEditor.FilloutData(tileCollection);

		GameObject tileSOCollectionObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/TileScriptableObjectCollection.prefab");
		TileScriptableObjectCollection tileSOCollection = tileSOCollectionObj.GetComponent<TileScriptableObjectCollection>();
		TileScriptableObjectCollectionEditor.FilloutData(tileSOCollection);

		GameObject explanationItemCollectionObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/ExplanationItemCollection.prefab");
		ExplanationItemCollection explanationItemCollection = explanationItemCollectionObj.GetComponent<ExplanationItemCollection>();
		ExplanationItemCollectionEditor.FilloutData(explanationItemCollection);
	}
}
