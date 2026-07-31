using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerChop : Card, IMessageReceiver
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Tile> warnedTiles;
	bool cancelled;

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
		warnedTiles = new List<Tile>(tilesAndDirection.tiles);
		TileGrid.AddWarnings(warnedTiles);

		cancelled = false;

		int chopTick = TurnControl.Instance.GetValue(owningCharacter);
		TurnEventController.Instance.AddEvent(Chop, chopTick);

		Finish();
	}

	void Cancel()
	{
		if (cancelled)
		{
			return;
		}

		cancelled = true;
		TileGrid.RemoveWarnings(warnedTiles);
	}

	void Chop()
	{
		if (cancelled)
		{
			TurnEventController.Instance.Pump();
			return;
		}

		if (!owningCharacter.alive)
		{
			TileGrid.RemoveWarnings(warnedTiles);
			TurnEventController.Instance.Pump();
			return;
		}

		AnimationController.Instance.ScrollToCharacter(owningCharacter, ChopActual, .5f);
	}

	void ChopActual()
	{
		TileGrid.RemoveWarnings(warnedTiles);

		List<Character> hitCharacters = GetTargetsOnTiles(warnedTiles);

		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			if (hitCharacters.Count == 0)
			{
				ShowNoTarget(owningCharacter.token.transform.position);
			}

			AttackCharacters(hitCharacters, new ActionController.AttackProfile(1, 6, 2));

			TurnEventController.Instance.Pump();
		});
	}	

	public override void OnCharacterKnockbackFinished(CharacterKnockbackFinishMessage message)
	{
		if (((CharacterKnockbackFinishMessage)message).knockedBackCharacter == owningCharacter)
		{
			Cancel();
		}
	}

	public override void OnCharacterDied(CharacterDiedMessage message)
	{
		if (((CharacterDiedMessage)message).character == owningCharacter)
		{
			Cancel();
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
		instructions.Add(new CardInstruction("If you are killed or knocked back first, the marks are cleared and nothing happens"));

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
