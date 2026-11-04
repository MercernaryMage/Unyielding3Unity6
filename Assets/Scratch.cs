using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Scratch : MonoBehaviour
{
}

/*

	public void Pump()
	{
		AnimationController.Instance.skipAnimations = false;
		processStarted = true;
		if (lastCharacter != null)
		{
			MovementController.Instance.HideMovement();
			TileGrid.Instance.HideAllTiles();
			if (!lastCharacter.hero)
			{
				AIController.Instance.Reshuffle(lastCharacter);
			}
			waitingEndTurnActions = lastCharacter.EndTurn();
			if (waitingEndTurnActions.Count > 0)
			{
				pumpingWaitingEndTurnActions = true;
				PumpWaitingEndTurnActions();
				return;
			}
		}
		FinishPump();


 * 
 * When EndTurn is called, it returns a list of pending actions.
 * We then go through each pending action one at a time, doing them
 * After each one is done, we return to EndTurn.
 * 
*/
