using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectIconRepository : Singleton<StatusEffectIconRepository>
{
	StatusEffectIconCollection iconsReal;
	Dictionary<string, Sprite> icons;

	private void Awake()
	{
		iconsReal = Resources.Load<GameObject>("StatusEffectIconCollection").GetComponent<StatusEffectIconCollection>();
		icons = new Dictionary<string, Sprite>();
		foreach (Sprite item in iconsReal.icons)
		{
			icons[item.name] = item;
		}
	}

	public Sprite GetExactIcon(string iconName)
	{
		if (icons.ContainsKey(iconName))
		{
			return icons[iconName];
		}
		return iconsReal.missingIcon;
	}
}
