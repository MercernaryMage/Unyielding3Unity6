using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;
using System.Linq.Expressions;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class TurnControl : SceneSingleton<TurnControl>
{
	List<TurnControlEntry> turnControlEntries = new List<TurnControlEntry>();

	public Character lastCharacter = null;

	public void AddCharacters(List<Character> characters)
	{
		DebugCharacterTurnOrder explicitOrder = FindFirstObjectByType<DebugCharacterTurnOrder>();
		if (explicitOrder != null && explicitOrder.useExplicitOrder)
		{
			// Assign descending values from the configured order so the sort below
			// preserves it (highest value goes first) instead of using initiative.
			List<Character> ordered = explicitOrder.GetOrderedCharacters();
			int value = ordered.Count;
			foreach (Character character in ordered)
			{
				TurnControlEntry turnControlEntry = new TurnControlEntry();
				turnControlEntry.character = character;
				turnControlEntry.value = value;
				--value;

				turnControlEntries.Add(turnControlEntry);
			}
		}
		else
		{
			foreach (Character character in characters)
			{
				TurnControlEntry turnControlEntry = new TurnControlEntry();
				turnControlEntry.character = character;
				turnControlEntry.value = character.GetInitiative();

				turnControlEntries.Add(turnControlEntry);
			}
		}
		turnControlEntries = turnControlEntries.OrderByDescending(o => o.value).ToList();
		UpdateSystem();
	}

	public void AddCharacter(Character c)
	{
		TurnControlEntry turnControlEntry = new TurnControlEntry();
		turnControlEntry.character = c;
		turnControlEntry.value = c.GetInitiative();


		turnControlEntries.Add(turnControlEntry);
		turnControlEntries = turnControlEntries.OrderByDescending(o => o.value).ToList();
		int index = turnControlEntries.IndexOf(turnControlEntry);
		if (index != -1 && index != turnControlEntries.Count - 1)
		{
			turnControlEntry.hasGone = true;
		}
		UpdateSystem();
	}

	public void UpdateSystem()
	{
		List<TurnControlEntry> turnControlDisplayOrder = new List<TurnControlEntry>(turnControlEntries);

		turnControlDisplayOrder.RemoveAll(o => !o.character.alive);

		for (int i = 0; i < turnControlDisplayOrder.Count; ++i)
		{
			if (turnControlDisplayOrder[0].hasGone)
			{
				TurnControlEntry entry = turnControlDisplayOrder[0];
				turnControlDisplayOrder.RemoveAt(0);
				turnControlDisplayOrder.Add(entry);
			}
			else
			{
				break;
			}
		}
		if (lastCharacter)
		{
			int last = turnControlDisplayOrder.Count - 1;
			if (turnControlDisplayOrder[last].character == lastCharacter)
			{
				TurnControlEntry entry = turnControlDisplayOrder[last];
				turnControlDisplayOrder.RemoveAt(last);
				turnControlDisplayOrder.Insert(0, entry);
			}
		}
		TurnControlDisplay.Instance.Set(turnControlDisplayOrder);
	}

	public void EndTurnClicked()
	{
		if (!BattleController.playerHasControl)
		{
			return;
		}
		if (ActionController.Instance.running)
		{
			ActionController.Instance.CancelAttackFromEndTurn();
		}
		Pump();
	}

	public void Pump()
	{
		if (lastCharacter != null)
		{
			CharacterEndTurnMessage characterEndTurnMessage = new CharacterEndTurnMessage();
			characterEndTurnMessage.character = lastCharacter;
			MessagePump.Instance.SendMessage(characterEndTurnMessage);
			TileGrid.Instance.HideAllTiles();
			if (!lastCharacter.hero)
			{
				AIController.Instance.Reshuffle(lastCharacter);
			}
		}
		for (int i = 0; i < turnControlEntries.Count; ++i)
		{
			if (!turnControlEntries[i].hasGone && turnControlEntries[i].character.alive)
			{
				turnControlEntries[i].hasGone = true;
				lastCharacter = turnControlEntries[i].character;
				UpdateSystem();
				PreTakeTurn(turnControlEntries[i].character);
				return;
			}
		}
		//everyone has gone, reset!
		bool found = false;
		for (int i = 0; i < turnControlEntries.Count; ++i)
		{
			turnControlEntries[i].hasGone = false;
			if (turnControlEntries[i].character.alive)
			{
				found = true;
			}
		}
		if (!found)
		{
			return;
		}
		Pump();
	}

	List<Object> turnStartLocks = new List<Object>();
	Character currentCharacter;

	public void RemoveLock(Object obj)
	{
		turnStartLocks.Remove(obj);
		if (turnStartLocks.Count == 0)
		{
			TakeTurnActual();
		}
	}

	public void TakeTurn()
	{
		currentCharacter.StartTurn();
		if (currentCharacter.hero && currentCharacter.gameObject.GetComponent<Exhausted>() != null)
		{
			StratagemDisplay.Instance.Set(currentCharacter);
		}
		if (currentCharacter.gameObject.GetComponent<Stasis>() == null)
		{
			CharacterStartTurnMessage characterStartTurnMessage = new CharacterStartTurnMessage();
			characterStartTurnMessage.character = currentCharacter;
			MessagePump.Instance.SendMessage(characterStartTurnMessage);

			turnStartLocks = characterStartTurnMessage.turnStartLocks;
		}

		if (turnStartLocks.Count == 0)
		{
			TakeTurnActual();
		}
	}

	public void PreTakeTurn(Character c)
	{
		currentCharacter = c;
		float runTime = SelectionManager.Instance.SnapCameraToCharacter(c) + .5f;
		Invoke("TakeTurn", runTime);
	}

	public void TakeTurnActual()
	{
		if (!currentCharacter.hero)
		{
			HeroDisplayRouter.Instance.Hide(true);
			BattleController.playerHasControl = false;
			AIController.Instance.TakeTurn(currentCharacter);
		}
		else
		{
			BattleController.playerHasControl = true;
			HeroDisplayRouter.Instance.Set(currentCharacter, true);

			MovementController.Instance.ShowMovement(currentCharacter);
			UIController.Instance.ShowHero(currentCharacter, true);
		}
	}
}



public class TurnControlEntry
{
	public int value;
	public Character character;
	public bool hasGone;
}