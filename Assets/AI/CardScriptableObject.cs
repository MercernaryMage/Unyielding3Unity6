using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public string className;
    public string cardDisplayName;
    public List<Tag> tags;

	public string GetTagStringValue(string tagName)
	{
		tagName = tagName.ToLower();
		foreach (Tag t in tags)
		{
			if (t.tagName.ToLower() == tagName)
			{
				return t.stringData;
			}
		}

		Debug.LogWarning("Tag not found!");
		return "";
	}

	public int GetTagIntValue(string tagName)
	{
		tagName = tagName.ToLower();
		foreach (Tag t in tags)
		{
			if (t.tagName.ToLower() == tagName)
			{
				return t.intData;
			}
		}

		Debug.LogWarning("Tag not found!");
		return -1;
	}

	public bool GetTagBoolValue(string tagName)
	{
		tagName = tagName.ToLower();
		foreach (Tag t in tags)
		{
			if (t.tagName.ToLower() == tagName)
			{
				return t.boolData;
			}
		}

		Debug.LogWarning($"Tag {tagName} not found!");
		return false;
	}
}
