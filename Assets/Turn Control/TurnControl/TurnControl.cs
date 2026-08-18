using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;
using System.Linq.Expressions;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;

public class TurnControl : SceneSingleton<TurnControl>
{
	List<TurnControlEntry> turnControlEntries = new List<TurnControlEntry>();

	public Character lastCharacter = null;

	public void AddCharacters(List<Character> characters)
	{
		DebugCharacterTurnOrder debugCharacterTurnOrder = FindFirstObjectByType<DebugCharacterTurnOrder>();
		List<Character> ordered;
		if (debugCharacterTurnOrder != null && debugCharacterTurnOrder.enabled)
		{
			ordered = debugCharacterTurnOrder.GetOrderedCharacters();
		}
		else
		{
			ordered = BuildAlternatingOrder(characters);
		}

		// Assign descending values from the configured order so the sort below
		// preserves it (highest value goes first) instead of using initiative.
		int value = ordered.Count;
		foreach (Character character in ordered)
		{
			TurnControlEntry turnControlEntry = new TurnControlEntry();
			turnControlEntry.character = character;
			turnControlEntry.value = value;
			--value;

			turnControlEntries.Add(turnControlEntry);
		}
		turnControlEntries = turnControlEntries.OrderByDescending(o => o.value).ToList();
		UpdateSystem();
	}

	List<Character> BuildAlternatingOrder(List<Character> characters)
	{
		Dictionary<Character, int> initiatives = new Dictionary<Character, int>();
		List<Character> heroes = new List<Character>();
		List<Character> enemies = new List<Character>();

		foreach (Character character in characters)
		{
			initiatives[character] = character.GetInitiative();
			if (character.hero)
			{
				heroes.Add(character);
			}
			else
			{
				enemies.Add(character);
			}
		}

		heroes = heroes.OrderByDescending(o => initiatives[o]).ToList();
		enemies = enemies.OrderByDescending(o => initiatives[o]).ToList();

		List<Character> ordered = new List<Character>();
		for (int i = 0; i < Mathf.Max(heroes.Count, enemies.Count); ++i)
		{
			if (i < heroes.Count)
			{
				ordered.Add(heroes[i]);
			}
			if (i < enemies.Count)
			{
				ordered.Add(enemies[i]);
			}
		}

		return ordered;
	}

	public void AddCharacter(Character c)
	{
		int lowestValue = 1;
		if (turnControlEntries.Count > 0)
		{
			lowestValue = turnControlEntries.Min(o => o.value);
		}

		TurnControlEntry turnControlEntry = new TurnControlEntry();
		turnControlEntry.character = c;
		turnControlEntry.value = lowestValue - 1;

		turnControlEntries.Add(turnControlEntry);
		turnControlEntries = turnControlEntries.OrderByDescending(o => o.value).ToList();
		if (turnControlEntry.value <= 0)
		{
			RenumberEntries();
		}
		UpdateSystem();
	}

	void RenumberEntries()
	{
		Dictionary<int, int> remappedValues = new Dictionary<int, int>();
		int value = turnControlEntries.Count;
		foreach (TurnControlEntry turnControlEntry in turnControlEntries)
		{
			remappedValues[turnControlEntry.value] = value;
			turnControlEntry.value = value;
			--value;
		}
		TurnEventController.Instance.RemapTicks(remappedValues);
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
		if (!BattleController.playerHasControl || processStarted)
		{
			return;
		}
		processStarted = true;
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
				//Every turn event on this tick resolves before the character gets to go. The events
				//are not inline, so the turn continues in TurnEventControlFinished().
				pendingCharacter = turnControlEntries[i].character;
				TurnEventController.Instance.PumpStart(turnControlEntries[i].value);
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
	public Character currentCharacter;
	Character pendingCharacter;

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
		processStarted = false;
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

	bool processStarted = false;

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

	public void TurnEventControlFinished()
	{
		Character c = pendingCharacter;
		pendingCharacter = null;
		if (c == null)
		{
			return;
		}
		//An event may have killed the character whose turn it was, so hand off to the next one.
		if (!c.alive)
		{
			UpdateSystem();
			Pump();
			return;
		}
		PreTakeTurn(c);
	}

	public int GetValue(Character c)
	{
		foreach (TurnControlEntry turnControlEntry in turnControlEntries)
		{
			if (turnControlEntry.character == c)
			{
				return turnControlEntry.value;
			}
		}
		return 0;
	}
}



public class TurnControlEntry
{
	public int value;
	public Character character;
	public bool hasGone;
}