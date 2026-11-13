using UnityEngine;

static public class MapFuncs
{
	public static void DaneTutorialFight()
	{
		TurnControl.Instance.PromoteCharacterToFirst("Dane");

		Character c = BattleController.Instance.enemies[2];
		if (c.reactions[0].normalReaction.GetCardName() != "Yelp")
		{
			Reaction r = c.reactions[1];
			c.reactions[1] = c.reactions[0];
			c.reactions[0] = r;
		}
	}

	public static void DavidTutorialFight()
	{
		TurnControl.Instance.PromoteCharacterToFirst("David");
	}

	public static void ShowLOSTutorial()
	{
		if (!PersistenceManager.Instance.GetFlag("ShowLOSTutorial"))
		{
			PersistenceManager.Instance.SetFlag("ShowLOSTutorial", true);
			CombatSystemMessage.Instance.Show("Line of Sight", "You can view what tiles can be seen from a location by holding 'z'");
		}
	}
}
