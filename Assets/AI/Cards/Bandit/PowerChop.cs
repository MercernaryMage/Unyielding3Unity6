using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerChop : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		route = Util.FindSmallestRoute(routes, null);
		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	public void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	public void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		tilesAndDirection = TemplateLibrary.Instance.ConeTargeting(owningCharacter, 2);
		if (tilesAndDirection == null)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "No target");
			Finish();
			return;
		}

		owningCharacter.SetFacing(tilesAndDirection.direction);
		List<Tile> warnedTiles = new List<Tile>(tilesAndDirection.tiles);
		foreach (Tile t in warnedTiles)
		{
			t.AddWarning();
		}

		int chopTick = TurnControl.Instance.GetValue(owningCharacter);
		TurnEventController.Instance.AddEvent(() => Chop(warnedTiles), chopTick);

		Finish();
	}

	void Chop(List<Tile> warnedTiles)
	{
		if (!owningCharacter.alive)
		{
			ClearWarnings(warnedTiles);
			TurnEventController.Instance.Pump();
			return;
		}

		AnimationController.Instance.ScrollToCharacter(owningCharacter, () => ChopActual(warnedTiles), .5f);
	}

	void ChopActual(List<Tile> warnedTiles)
	{
		ClearWarnings(warnedTiles);

		List<Character> hitCharacters = new List<Character>();
		foreach (Tile t in warnedTiles)
		{
			if (t.character != null && !hitCharacters.Contains(t.character))
			{
				hitCharacters.Add(t.character);
			}
		}

		NoFriendlyFire noFriendlyFire = owningCharacter.gameObject.GetComponent<NoFriendlyFire>();
		if (noFriendlyFire != null)
		{
			noFriendlyFire.SpareAllies(hitCharacters);
		}

		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			if (hitCharacters.Count == 0)
			{
				FloatingCombatNumberController.Instance.ShowFloatingCombatNumber(
					owningCharacter.token.transform.position + Vector3.up * 1.5f, "no target");
			}

			foreach (Character c in hitCharacters)
			{
				ActionController.Instance.AttackCharacter(c, owningCharacter, new ActionController.AttackProfile(1, 6, 2));
			}

			TurnEventController.Instance.Pump();
		});
	}

	void ClearWarnings(List<Tile> warnedTiles)
	{
		foreach (Tile t in warnedTiles)
		{
			t.RemoveWarning();
		}
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Mark a range 2 cone covering as many enemies as possible"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("At the start of your next turn, deal 1d6+2 damage to any character on a marked tile"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(4, 3),
			new Tuple<int, int>(5, 3),
			new Tuple<int, int>(6, 3),
			new Tuple<int, int>(3, 2),
			new Tuple<int, int>(4, 2),
			new Tuple<int, int>(5, 2),
			new Tuple<int, int>(6, 2),
			new Tuple<int, int>(7, 2),
		});
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
