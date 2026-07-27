using System.Collections.Generic;
using UnityEngine;

public class Incoming : StatusEffect
{
	List<Tile> outlinedTiles = new List<Tile>();
	int outlineId = -1;

	public void Set(List<Tile> tiles, int id)
	{
		outlinedTiles = tiles;
		outlineId = id;
	}

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != character)
		{
			return;
		}

		//Hit every character standing on an outlined tile, once each.
		List<Character> hitCharacters = new List<Character>();
		foreach (Tile t in outlinedTiles)
		{
			if (t.character != null && !hitCharacters.Contains(t.character))
			{
				hitCharacters.Add(t.character);
			}
		}

		foreach (Character c in hitCharacters)
		{
			ActionController.Instance.AttackCharacter(c, character, new ActionController.AttackProfile(1, 6, 3));
		}

		//OnDestroy -> EffectBeingRemoved clears the outline.
		Destroy(this);
	}

	public override void EffectBeingRemoved()
	{
		if (outlineId != -1)
		{
			OutlineManager.Instance.Destroy(outlineId);
			outlineId = -1;
		}
	}

	public override string GetExplanationName()
	{
		return "Incoming";
	}
}
