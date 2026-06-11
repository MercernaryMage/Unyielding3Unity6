using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTag : MonoBehaviour
{
    public List<string> tags = new List<string>();

	public static bool HasTag(GameObject obj, string tagName)
	{
		UTag tag = obj.GetComponent<UTag>();
		if (tag == null)
		{
			return false;
		}
		foreach (string t in tag.tags)
		{
			if (t == tagName)
			{
				return true;
			}
		}
		return false;
	}
}
