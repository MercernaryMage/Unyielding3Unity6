using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : SceneSingleton<BattleController>
{
	public List<Character> heroes;
	public List<Character> enemies;

	public GameObject warningPrefab;
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
		warning.transform.position = camera.WorldToScreenPoint(tile.transform.position) - new Vector3(0, 25, 0);
		float scale = referenceOrthographicSize / camera.orthographicSize;
		warning.transform.localScale = new Vector3(scale, scale, 1f);
	}

	public void ShowWarning(Tile tile, string text)
	{
		ActiveWarning activeWarning = new ActiveWarning();
		activeWarning.tile = tile;
		activeWarning.warning = Instantiate(warningPrefab, UIController.Instance.worldUI);
		activeWarning.warning.GetComponent<TileWarning>().SetText(text);
		activeWarning.warning.SetActive(true);
		PositionTileWarning(activeWarning.warning, tile);
		activeWarnings.Add(activeWarning);
	}

	public void ShowCharacterWarning(Character c, string text)
	{
		ShowWarning(TileGrid.Instance.FindCharacter(c)[0], text);
	}

	public void HideWarnings()
	{
		foreach (ActiveWarning activeWarning in activeWarnings)
		{
			Destroy(activeWarning.warning);
		}
		activeWarnings.Clear();
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

		Character placedCharacter = MapBuilder.PlaceHero(t.x, t.y, storageCharacter, GetPlacementFacing(t));
		placedCharacters[t] = placedCharacter;

		UIController.Instance.ShowHero(placedCharacter, false);
	}

	public void StartFight()
	{
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

		if (UIController.Instance.heroDisplay.lastCharacter == character)
		{
			UIController.Instance.heroDisplay.lastCharacter = null;
			UIController.Instance.HideHero();
		}

		if (character.storageCharacter != null)
		{
			character.storageCharacter.createdCharacter = null;
		}

		Destroy(character.token.gameObject);
		Destroy(character.gameObject);
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
