using System.Collections.Generic;
using UnityEngine;

public class FollowingBlast : Blast
{
	public override void Execute()
	{
		Character target = GetLowestMaxHPTarget();
		if (target == null)
		{
			Debug.Log("No target");
			Finish();
			return;
		}

		Following following = (Following)target.AddStatusEffect(typeof(Following), null);
		following.Track(2);

		int detonationTick = TurnControl.Instance.GetValue(owningCharacter);
		TurnEventController.Instance.AddEvent(() => DetonateFollowing(following), detonationTick);

		AnimationController.Instance.ScrollToCharacter(target, () => Finish(), .5f);
	}

	void DetonateFollowing(Following following)
	{
		if (following == null)
		{
			TurnEventController.Instance.Pump();
			return;
		}

		List<Tile> characterTiles = TileGrid.Instance.FindCharacter(following.character);
		if (characterTiles.Count == 0)
		{
			following.Remove();
			TurnEventController.Instance.Pump();
			return;
		}

		Tile centerTile = characterTiles[0];
		AnimationController.Instance.ScrollToTile(centerTile, () => DetonateFollowingActual(following, centerTile), .5f);
	}

	void DetonateFollowingActual(Following following, Tile centerTile)
	{
		List<Character> hitCharacters = new List<Character>();
		foreach (Tile t in following.GetTiles())
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

		if (hitCharacters.Count == 0)
		{
			FloatingCombatNumberController.Instance.ShowFloatingCombatNumber(
				centerTile.transform.position + Vector3.up * 1.5f, "no target");
		}

		foreach (Character c in hitCharacters)
		{
			ActionController.Instance.AttackCharacter(c, owningCharacter, new ActionController.AttackProfile(1, 6, 3));
		}

		following.Remove();

		TurnEventController.Instance.Pump();
	}

	public static new List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Target the enemy with the lowest max HP"));
		instructions.Add(new CardInstruction("Mark the tiles within range 2 of the target. The marks follow them as they move"));
		instructions.Add(new CardInstruction("At the start of your next turn, deal 1d6+3 damage to any character on a marked tile"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
