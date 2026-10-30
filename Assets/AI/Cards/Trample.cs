using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trample : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	int range = 4;
	List<Character> hitCharacters;

	public const float moveTimePerTile = .2f;

	public override void Execute()
	{
		tilesAndDirection = TemplateLibrary.Instance.GetMostCharactersInCardinalDirections(owningCharacter, range);

		if (!TileGrid.TilesContainHero(tilesAndDirection.tiles))
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "no target");
			Finish();
			return;
		}

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles);
	}

	public void GoToFinish()
	{
		Finish();
	}

	public void ReturnFromShowingTiles()
	{
		DoTrample(owningCharacter, tilesAndDirection, range, GoToFinish, GetIntValue("Damage"));
	}

	public static List<Tile> GetTramplePath(Character character, Tile startTile, Tuple<int, int> dir, int range)
	{
		List<Tile> outTiles = new List<Tile>();
		for (int i = 1; i <= range; ++i)
		{
			Tile newTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * i, startTile.y + dir.Item2 * i);
			if (newTile == null)
			{
				break;
			}
			if (TileGrid.Instance.WhatTilesWouldCharacterTake(character, newTile) == null)
			{
				break;
			}
			outTiles.Add(newTile);
		}
		return outTiles;
	}

	public static void DoTrample(Character character, TemplateLibrary.TilesAndDirection tilesAndDirection, int range, Action callback, int maxDamage)
	{
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(character);
		Tile startTile = characterOrigin[0];
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];

		List<Tile> trail = GetTramplePath(character, startTile, dir, range);
		List<Character> hitCharacters = new List<Character>();

		foreach (Tile t in tilesAndDirection.tiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		List<Tile> actualSweptTiles = TemplateLibrary.Instance.GetTilesInMatchedSizeCardinalDirection(character, trail.Count, (int)tilesAndDirection.direction);
		foreach (Tile t in actualSweptTiles)
		{
			if (t.character != null && t.character.hero && !hitCharacters.Contains(t.character))
			{
				hitCharacters.Add(t.character);
			}
		}

		Action damageHitCharacters = () =>
		{
			foreach (Character c in hitCharacters)
			{
				if (c.GetComponent<Stasis>() != null)
				{
					continue;
				}
				ActionController.AttackResults results = new ActionController.AttackResults();
				results.fakeHit = true;
				ActionController.Instance.DamageCharacter(c, character, new ActionController.AttackProfile(1, maxDamage, 0), results);
				CombatLogControl.Instance.AddEntry($"Trample deals {results.damageDealt} to {c.displayName}");
				c.AddStatusEffect(typeof(Stun), null);
			}
			callback();
		};

		if (trail.Count == 0)
		{
			damageHitCharacters();
			return;
		}

		character.SetFacing(TileGrid.Instance.GetFacingDirection(startTile, trail[trail.Count - 1]));

		MoveAttackerThroughPath(character, trail, 0, damageHitCharacters);
	}

	public static void MoveAttackerThroughPath(Character character, List<Tile> path, int index, Action onComplete)
	{
		if (index >= path.Count)
		{
			onComplete();
			return;
		}

		Tile tile = path[index];

		GlobalPositionLerp positionLerp = character.token.gameObject.AddComponent<GlobalPositionLerp>();
		positionLerp.p0 = character.token.transform.position;
		positionLerp.p1 = GetTokenPositionForTile(character, tile);
		positionLerp.type = InterpolationType.Simple;
		positionLerp.runTime = moveTimePerTile;
		positionLerp.callbackFunction = () =>
		{
			ShoveCharactersAtTile(character, tile, () =>
			{
				TileGrid.Instance.MoveCharacterToTile(character, tile);
				MoveAttackerThroughPath(character, path, index + 1, onComplete);
			});
		};
		positionLerp.Init();
	}

	static Vector3 GetTokenPositionForTile(Character character, Tile tile)
	{
		Vector3 offset = new Vector3(character.characterDefinition.size - 1, 0, character.characterDefinition.size - 1);
		return tile.transform.parent.TransformPoint(offset);
	}

	static void ShoveCharactersAtTile(Character character, Tile tile, Action onComplete)
	{
		List<Tile> impactTiles = TileGrid.Instance.WhatTilesWouldCharacterTake(character, tile);
		List<Character> impactedCharacters = new List<Character>();

		if (impactTiles != null)
		{
			foreach (Tile impactTile in impactTiles)
			{
				if (impactTile.character == null || impactTile.character == character)
				{
					continue;
				}
				if (impactedCharacters.Contains(impactTile.character))
				{
					continue;
				}
				impactedCharacters.Add(impactTile.character);
			}
		}

		int pendingMoves = impactedCharacters.Count;
		if (pendingMoves == 0)
		{
			onComplete();
			return;
		}

		bool movesStarted = false;

		Action tryFinish = () =>
		{
			if (!movesStarted || pendingMoves > 0)
			{
				return;
			}
			onComplete();
		};

		foreach (Character impacted in impactedCharacters)
		{
			ActionController.Instance.MoveCharacterFromImpact(impacted, impactTiles, () =>
			{
				--pendingMoves;
				tryFinish();
			});
		}

		movesStarted = true;
		tryFinish();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject, Character character )
	{
		DisplayGrid.Instance.Clear(10, 10); 
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Select direction that would hit most enemies"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Move in that direction"));
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target"));
		instructions.Add(new CardInstruction("<u>Stunned</u> on hit"));
		
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2Enemy, DisplayGrid.DisplayGridDirection.South, 4, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.South, 4, 0);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.North, 4, 8);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.West, 0, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.East, 8, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(4,6),
			new Tuple<int, int>(5,6),
			new Tuple<int, int>(4,7),
			new Tuple<int, int>(5,7),

			new Tuple<int, int>(4,2),
			new Tuple<int, int>(5,2),
			new Tuple<int, int>(4,3),
			new Tuple<int, int>(5,3),

			new Tuple<int, int>(2,5),
			new Tuple<int, int>(3,5),
			new Tuple<int, int>(2,4),
			new Tuple<int, int>(3,4),

			new Tuple<int, int>(6,5),
			new Tuple<int, int>(7,5),
			new Tuple<int, int>(6,4),
			new Tuple<int, int>(7,4),
		});
		DisplayGrid.Instance.Show();




		return instructions;
	}
}
