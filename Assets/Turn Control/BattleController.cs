using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Reflection;
using Mono.Cecil;

public class BattleController : SceneSingleton<BattleController>
{
	public List<Character> heroes;
	public List<Character> enemies;

	public GameObject warningPrefab;
	public GameObject AoOIndicatorPrefab;
	public float referenceOrthographicSize = 10f;

	static public bool playerHasControl = true;
	static Character storedCharacterForControl;

	public bool doingPlacement = false;
	bool playerWinning = false;
	bool enemyWinning = false;

	List<PropDucker> duckers = new List<PropDucker>();
	bool duckersRequested;

	Dictionary<Tile, Direction> tilesForPlacement = new Dictionary<Tile, Direction>();

	Dictionary<Tile, Character> placedCharacters = new Dictionary<Tile, Character>();

	public GameObject EndTurnButton;
	public GameObject deployButton;

	class ActiveWarning
	{
		public Tile tile;
		public GameObject warning;
	}

	class ActiveAoOIndicator
	{
		public Character attacker;
		public Character defender;
		public GameObject indicator;
		public float height;
		public float length;
	}

	List<ActiveAoOIndicator> activeAoOIndicators = new List<ActiveAoOIndicator>();

	List<ActiveWarning> activeWarnings = new List<ActiveWarning>();

	public void Start()
	{
		if (!BattleController.Instance.doingPlacement)
		{
			Invoke("StartCombat", 1);
		}
	}

	public void StartCombat()
	{
		//CALL CODE HERE
		if (!string.IsNullOrEmpty(MapBuilder.Instance.lastMapFunc))
		{
			Type type = typeof(MapFuncs);
			MethodInfo method = type.GetMethod(MapBuilder.Instance.lastMapFunc, BindingFlags.Public | BindingFlags.Static);
			method.Invoke(null, null);
		}
		if (doingPlacement)
		{
			return;
		}
		ImposedControl.Instance.SetDirty();
		ImposedControl.Instance.Run();
		MessagePump.Instance.SendMessage(new CombatStartMessage());
		TurnControl.Instance.Pump();
	}

	

	void PositionTileWarning(GameObject warning, Tile tile)
	{
		Camera camera = Camera.main;
		float scale = referenceOrthographicSize / camera.orthographicSize;
		warning.transform.position = camera.WorldToScreenPoint(tile.transform.position) - new Vector3(0, 25, 0) * scale;
		warning.transform.localScale = new Vector3(scale, scale, 1f);
	}

	public void ShowWarning(Tile tile, string text, string tooltipTitle, string tooltipText)
	{
		ActiveWarning activeWarning = new ActiveWarning();
		activeWarning.tile = tile;
		activeWarning.warning = Instantiate(warningPrefab, UIController.Instance.worldUI);
		activeWarning.warning.GetComponent<TileWarning>().SetText(text, tooltipTitle, tooltipText);
		activeWarning.warning.SetActive(true);
		PositionTileWarning(activeWarning.warning, tile);
		activeWarnings.Add(activeWarning);
	}

	public void ShowCharacterWarning(Character c, string text, string tooltipTitle, string tooltipText)
	{
		ShowWarning(TileGrid.Instance.FindCharacter(c)[0], text, tooltipTitle, tooltipText);
	}

	public void HideWarnings()
	{
		foreach (ActiveWarning activeWarning in activeWarnings)
		{
			Destroy(activeWarning.warning);
		}
		activeWarnings.Clear();
	}

	public void ShowAoOIndicator(Character attacker, Character defender)
	{
		if (attacker == null || defender == null || attacker.token == null || defender.token == null)
		{
			return;
		}

		ActiveAoOIndicator activeAoOIndicator = new ActiveAoOIndicator();
		activeAoOIndicator.attacker = attacker;
		activeAoOIndicator.defender = defender;
		activeAoOIndicator.indicator = Instantiate(AoOIndicatorPrefab);
		activeAoOIndicator.height = activeAoOIndicator.indicator.transform.position.y;
		activeAoOIndicator.length = activeAoOIndicator.indicator.transform.localScale.x;

		PositionAoOIndicator(activeAoOIndicator);
		activeAoOIndicators.Add(activeAoOIndicator);
	}

	public void HideAoOIndicators()
	{
		foreach (ActiveAoOIndicator activeAoOIndicator in activeAoOIndicators)
		{
			Destroy(activeAoOIndicator.indicator);
		}
		activeAoOIndicators.Clear();
	}

	void PositionAoOIndicator(ActiveAoOIndicator activeAoOIndicator)
	{
		Vector3 attackerPosition = activeAoOIndicator.attacker.token.transform.position;
		Vector3 defenderPosition = activeAoOIndicator.defender.token.transform.position;
		attackerPosition.y = activeAoOIndicator.height;
		defenderPosition.y = activeAoOIndicator.height;

		Vector3 difference = defenderPosition - attackerPosition;
		float distance = difference.magnitude;

		activeAoOIndicator.indicator.transform.position = attackerPosition + difference * .5f;
		activeAoOIndicator.indicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, -difference / distance);
		activeAoOIndicator.indicator.transform.localScale = new Vector3(activeAoOIndicator.length,
			distance * .5f,
			activeAoOIndicator.length);
	}

	void PositionAoOIndicators()
	{
		foreach (ActiveAoOIndicator activeAoOIndicator in activeAoOIndicators)
		{
			PositionAoOIndicator(activeAoOIndicator);
		}
	}

	void PositionWarnings()
	{
		foreach (ActiveWarning activeWarning in activeWarnings)
		{
			if (activeWarning.warning != null && activeWarning.tile != null)
			{
				PositionTileWarning(activeWarning.warning, activeWarning.tile);
			}
		}
	}

	public void AddCharacter(Character c)
	{
		if (c.hero)
		{
			heroes.Add(c);
		}
		else
		{
			enemies.Add(c);
		}
	}

	public void HealCharacter(Character c, int healValue)
	{
		c.currentHP = Mathf.Min(c.currentHP + healValue, c.maxHP);
	}

	public void HandleSurge(Character c)
	{
		int cost = 1;

		if (c.storageCharacter.surgeIndex == 1)
		{
			cost = Util.RollDice(1, 3);
		}
		else if (c.storageCharacter.surgeIndex >= 2)
		{
			int addedCost = c.storageCharacter.surgeIndex - 2;
			addedCost = Mathf.Min(addedCost, 4);
			cost = Util.RollDice(1, 6) + addedCost;
		}

		c.SpendEnergy(cost);
		++c.storageCharacter.surgeIndex;
		c.actionCount += 2;
		c.ResetActions(false);
	}

	public void HandleDash(Character c)
	{
		c.AddMovement();
	}

	public void RequestRunDuckers()
	{
		duckersRequested = true;
	}

	void Update()
	{
		PositionWarnings();
		PositionAoOIndicators();

		if (duckersRequested)
		{
			duckersRequested = false;
			RunDuckers();
		}

		if (playerWinning || enemyWinning)
		{
			return;
		}
		bool found = false;

		//check for hero victory
		foreach (Character c in enemies)
		{
			if (c.alive && !c.characterDefinition.isMinion)
			{
				found = true;
			}
		}
		if (!found)
		{
			playerWinning = true;
			WinLossControl.Instance.PlayerWins();
			return;
		}

		//check for enemy victory
		foreach (Character c in heroes)
		{
			if (c.alive)
			{
				found = true;
			}
		}
		if (!found)
		{
			enemyWinning = true;
			WinLossControl.Instance.EnemyWins();
			return;
		}

		deployButton.SetActive(doingPlacement && placedCharacters.Count == 4);
	}

	public void TileClicked(Tile t, bool rightClicked)
	{
		if (rightClicked)
		{
			RemovePlacedCharacter(t);
			return;
		}

		if (placedCharacters.ContainsKey(t) || t.character != null)
		{
			return;
		}

		StorageCharacter storageCharacter = GetNextCharacterToPlace();
		if (storageCharacter == null)
		{
			return;
		}

		placedCharacters[t] = MapBuilder.PlaceHero(t.x, t.y, storageCharacter, GetPlacementFacing(t));

		ShowNextCharacterToPlace();
	}

	void ShowNextCharacterToPlace()
	{
		if (!doingPlacement)
		{
			return;
		}

		StorageCharacter storageCharacter = GetNextCharacterToPlace();
		if (storageCharacter == null)
		{
			UIController.Instance.heroDisplay.lastCharacter = null;
			UIController.Instance.HideHero();
			return;
		}

		UIController.Instance.ShowHeroPreview(storageCharacter);
	}

	public void StartFight()
	{
		doingPlacement = false;

		Dictionary<StorageCharacter, Tile> placementTiles = new Dictionary<StorageCharacter, Tile>();
		foreach (KeyValuePair<Tile, Character> pair in placedCharacters)
		{
			placementTiles[pair.Value.storageCharacter] = pair.Key;
		}

		List<Tile> occupiedTiles = new List<Tile>(placedCharacters.Keys);
		foreach (Tile tile in occupiedTiles)
		{
			RemovePlacedCharacter(tile);
		}

		foreach (StorageCharacter storageCharacter in PersistenceManager.Instance.currentTeam)
		{
			if (!placementTiles.ContainsKey(storageCharacter))
			{
				continue;
			}

			Tile tile = placementTiles[storageCharacter];
			MapBuilder.PlaceHero(tile.x, tile.y, storageCharacter, GetPlacementFacing(tile));
		}

		foreach (KeyValuePair<Tile, Direction> pair in tilesForPlacement)
		{
			pair.Key.HideOverlay(Tile.OverlayType.Selected);
			pair.Key.HideOverlay(Tile.OverlayType.PossibleMovement);
		}
		tilesForPlacement.Clear();
		doingPlacement = false;

		List<Character> characters = new List<Character>();
		characters.AddRange(heroes);
		characters.AddRange(enemies);
		TurnControl.Instance.ClearCharacters();
		TurnControl.Instance.AddCharacters(characters);
		EndTurnButton.SetActive(true);

		playerHasControl = true;
		StartCombat();
	}

	StorageCharacter GetNextCharacterToPlace()
	{
		foreach (StorageCharacter storageCharacter in PersistenceManager.Instance.currentTeam)
		{
			if (!IsCharacterPlaced(storageCharacter))
			{
				return storageCharacter;
			}
		}
		return null;
	}

	bool IsCharacterPlaced(StorageCharacter storageCharacter)
	{
		foreach (KeyValuePair<Tile, Character> pair in placedCharacters)
		{
			if (pair.Value.storageCharacter == storageCharacter)
			{
				return true;
			}
		}
		return false;
	}

	void RemovePlacedCharacter(Tile t)
	{
		if (!placedCharacters.ContainsKey(t))
		{
			return;
		}

		Character character = placedCharacters[t];
		placedCharacters.Remove(t);

		TileGrid.Instance.RemoveCharacter(character);
		heroes.Remove(character);

		if (character.storageCharacter != null)
		{
			character.storageCharacter.createdCharacter = null;
		}

		Destroy(character.token.gameObject);
		Destroy(character.gameObject);

		ShowNextCharacterToPlace();
	}

	static public void ReturnControlToPlayer()
	{
		ActionController.Instance.ReturnFromReaction();
		UIController.Instance.heroDisplay.lastCharacter = TurnControl.Instance.lastCharacter;
		UIController.Instance.heroDisplay.UpdateWithLastCharacter();
	}

	static public void RemoveControlFromPlayer(Character character)
	{
		playerHasControl = false;
		storedCharacterForControl = character;
		UIController.Instance.heroDisplay.Hide(true);
	}

	public void AddDucker(PropDucker ducker)
	{
		duckers.Add(ducker);
	}

	public void RunDuckers()
	{
		foreach (PropDucker ducker in duckers)
		{
			ducker.waitingForHit = true;
			ducker.ourCollider.enabled = true;
		}

		foreach (Tile t in TileGrid.Instance.tiles)
		{
			t.PerformDuckCheck();
		}
		foreach (PropDucker ducker in duckers)
		{
			ducker.ourCollider.enabled = false;
		}
	}

	public List<Character> GetAllies(Character c)
	{
		return c.hero ? heroes : enemies;
	}

	public List<Character> GetEnemies(Character c)
	{
		return c.hero ? enemies : heroes;
	}

	public void CardFinished(float time)
	{
		Invoke("CardFinishedActual", .5f + time);
	}
	public void CardFinishedActual()
	{
		playerHasControl = true;
		TurnControl.Instance.Pump();
	}

	public void SetPlacementTiles(Dictionary<Tile, Direction> tiles)
	{
		EndTurnButton.SetActive(false);
		foreach (KeyValuePair<Tile, Direction> pair in tiles)
		{
			pair.Key.ShowOverlay(Tile.OverlayType.PossibleMovement);
		}
		tilesForPlacement = tiles;
		doingPlacement = true;

		ShowNextCharacterToPlace();
	}

	public bool IsPlacementTile(Tile t)
	{
		return tilesForPlacement.ContainsKey(t);
	}

	Direction GetPlacementFacing(Tile t)
	{
		if (tilesForPlacement.ContainsKey(t))
		{
			return tilesForPlacement[t];
		}
		return Direction.North;
	}
}
