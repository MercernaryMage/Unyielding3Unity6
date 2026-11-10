using UnityEngine;

static public class MapFuncs
{
	public static void DaneTutorialFight()
	{
		TurnControl.Instance.PromoteCharacterToFirst("Dane");
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
