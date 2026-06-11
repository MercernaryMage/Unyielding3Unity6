using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StratagemDisplay : SceneSingleton<StratagemDisplay>
{
	public GameObject content;

	public static bool stratagemDisplayUp;
	Character character;

	public void Set(Character c)
	{
		character = c;
		content.SetActive(true);
	}

	public void Close()
	{
		stratagemDisplayUp = false;
		content.SetActive(false);
	}

	public void ClickRest()
	{
		character.RestoreAllEnergy();
		Exhausted exhausted = character.gameObject.GetComponent<Exhausted>();
		if (exhausted != null)
		{
			Destroy(exhausted);
		}
		Close();
	}
}
