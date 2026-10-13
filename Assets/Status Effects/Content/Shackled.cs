using UnityEngine;

public class Shackled : StatusEffect
{
	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage)
	{
		Character mover = characterFinishedMovingMessage.movingCharacter;
		if (mover == character)
		{
			return;
		}
		if (mover.hero != character.hero)
		{
			return;
		}
		if (!TileGrid.Instance.CharactersAreAdjacent(mover, character))
		{
			return;
		}
		Destroy(this);
	}

	public override void EffectBeingRemoved()
	{
		Paralyzed paralyzed = character.GetComponent<Paralyzed>();
		if (paralyzed != null)
		{
			Destroy(paralyzed);
		}
	}

	public override string GetExplanationName()
	{
		return "Shackled";
	}

	public override string GetIconName()
	{
		return "Shackled";
	}

	public override bool StatusEffectIsVisibleInHealthBar()
	{
		return true;
	}
}
