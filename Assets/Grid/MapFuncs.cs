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
}
