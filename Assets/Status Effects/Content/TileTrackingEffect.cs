using System.Collections.Generic;
using UnityEngine;

public abstract class TileTrackingEffect : StatusEffect
{
	int range;
	List<Tile> warnedTiles = new List<Tile>();

	public void Track(int trackRange)
	{
		ClearWarnings();
		range = trackRange;
		warnedTiles = GetTiles();
		TileGrid.AddWarnings(warnedTiles);
	}

	public List<Tile> GetTiles()
	{
		return TileGrid.Instance.GetAllTilesInRangeOfCharacter(character, range);
	}

	public void Remove()
	{
		Destroy(this);
	}

	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage)
	{
		if (characterFinishedMovingMessage.movingCharacter == character)
		{
			Redraw();
		}
	}

	public override void OnCharacterKnockbackFinish(CharacterKnockbackFinishMessage message)
	{
		if (message.knockedBackCharacter == character)
		{
			Redraw();
		}
	}

	void Redraw()
	{
		if (warnedTiles.Count == 0)
		{
			return;
		}
		List<Tile> newTiles = GetTiles();
		TileGrid.AddWarnings(newTiles);
		ClearWarnings();
		warnedTiles = newTiles;
	}

	void ClearWarnings()
	{
		TileGrid.RemoveWarnings(warnedTiles);
		warnedTiles.Clear();
	}

	public override void EffectBeingRemoved()
	{
		ClearWarnings();
	}
}
