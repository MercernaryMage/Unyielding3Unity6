using UnityEngine;

public class Staked : StatusEffect
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
		Immobilize immobilize = character.GetComponent<Immobilize>();
		if (immobilize != null)
		{
			Destroy(immobilize);
		}
	}

	public override string GetExplanationName()
	{
		return "Staked";
	}
}
