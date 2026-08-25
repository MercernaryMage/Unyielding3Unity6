using System.Collections.Generic;
using UnityEngine;

public class ImposedControl : SceneSingleton<ImposedControl>
{
    bool isDirty;

	public void SetDirty()
	{
		isDirty = true;
	}

	public void Run()
	{
		if (!isDirty) 
		{
			return;
		}
		isDirty = false;
		SetEngagedForAllCharacters();
	}

	public void SetEngagedForAllCharacters()
	{
		SetEngagedForTeam(BattleController.Instance.heroes);
		SetEngagedForTeam(BattleController.Instance.enemies);
	}

	public void SetEngagedForTeam(List<Character> chracters)
	{
		foreach (Character hero in chracters)
		{
			if (!hero.alive)
			{
				continue;
			}
			bool found = HandleImposed(hero);
			if (!found)
			{
				Imposed imposed = hero.GetComponent<Imposed>();
				if (imposed != null)
				{
					Destroy(imposed);
				}
			}
		}
		HeroDisplayRouter.Instance.UpdateStatusEffects();
	}

	public bool HandleImposed(Character movingCharacter)
	{
		List<Character> adjacentCharacters = TileGrid.Instance.GetAllAdjacentCharcters(movingCharacter);
		bool found = false;
		foreach (Character character in adjacentCharacters)
		{
			if (character.hero != movingCharacter.hero)
			{
				character.AddStatusEffect(typeof(Imposed), null);
				movingCharacter.AddStatusEffect(typeof(Imposed), null);
				found = true;
			}
		}
		HeroDisplayRouter.Instance.UpdateStatusEffects();
		return found;
	}
}
