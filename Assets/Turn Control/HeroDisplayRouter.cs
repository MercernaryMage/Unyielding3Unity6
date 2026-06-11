using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HeroDisplayRouter : SceneSingleton<HeroDisplayRouter>
{
    public FancyHeroDisplay mainDisplay;
	public HeroDisplay subDisplay;

	public void Hide(bool main)
	{
		if (main)
		{
			mainDisplay.Hide(true);
		}
		else
		{
			subDisplay.Hide(false);
		}
	}

	public void Show(bool main)
	{
		if (main)
		{
			mainDisplay.Show();
		}
		else
		{
			subDisplay.Show();
		}
	}

	public void Set(Character character, bool main)
	{
		if (main)
		{
			mainDisplay.Set(character);
		}
		else
		{
			subDisplay.Set(character);
		}
	}

	public void UpdateWithLastCharacter(bool main)
	{
		if (main)
		{
			mainDisplay.UpdateWithLastCharacter();
		}
		else
		{
			subDisplay.UpdateWithLastCharacter();
		}
	}
}
