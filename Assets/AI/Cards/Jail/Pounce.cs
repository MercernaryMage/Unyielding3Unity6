using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pounce
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	int range = 2;
	List<Character> hitCharacters;
	Character owningCharacter;
	Action callback;

	public void Execute(Character o, Character targetCharacter, Action actionToStore)
	{
		callback = actionToStore;
		owningCharacter = o;
		tilesAndDirection = TemplateLibrary.Instance.GetPounceTemplate(owningCharacter, targetCharacter, range);

// 		if (!TileGrid.TilesContainHero(tilesAndDirection.tiles))
// 		{
// 			return;
// 		}

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles);
	}

	public void ReturnFromShowingTiles()
	{
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(owningCharacter);
		Tile startTile = characterOrigin[0];
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];

		List<Tile> trail = Trample.GetTramplePath(owningCharacter, startTile, dir, range);
		hitCharacters = new List<Character>();

		foreach (Tile t in tilesAndDirection.tiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);

			if (t.character != null && t.character.hero && !hitCharacters.Contains(t.character))
			{
				hitCharacters.Add(t.character);
			}
		}

		Action damageHitCharacters = () =>
		{
			foreach (Character c in hitCharacters)
			{
				ActionController.AttackResults results = new ActionController.AttackResults();
				ActionController.Instance.DamageCharacter(c, owningCharacter, new ActionController.AttackProfile(1, 4, 0), results);
				CombatLogControl.Instance.AddEntry($"Pounce deals {results.damageDealt} to {c.displayName}");
				c.AddStatusEffect(typeof(Stun), null);
			}
			callback.Invoke();
		};

		if (trail.Count == 0)
		{
			damageHitCharacters();
			return;
		}

		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(startTile, trail[trail.Count - 1]));

		Trample.MoveAttackerThroughPath(owningCharacter, trail, 0, damageHitCharacters);
	}
}
