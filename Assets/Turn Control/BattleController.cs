using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : SceneSingleton<BattleController>
{
	public List<Character> heroes;
	public List<Character> enemies;

	public GameObject warningPrefab;
	public float referenceOrthographicSize = 10f;

	static public bool playerHasControl = true;
	static Character storedCharacterForControl;

	bool playerWinning = false;
	bool enemyWinning = false;

	List<PropDucker> duckers = new List<PropDucker>();
	bool duckersRequested;

	Tile provokeWarningTile;
	GameObject provokeWarning;

	Tile mousedOverWarningTile;
	GameObject mousedOverWarning;

	Tile characterWarningTile;
	GameObject characterWarning;

	public void Start()
	{
		Invoke("StartCombat", 1);
	}

	public void StartCombat()
	{
		MessagePump.Instance.SendMessage(new CombatStartMessage());
		TurnControl.Instance.Pump();
	}

	public GameObject CreateTileWarning(Tile tile, string text)
	{
		GameObject warning = Instantiate(warningPrefab, UIController.Instance.worldUI);
		warning.GetComponent<TileWarning>().SetText(text);
		warning.SetActive(true);
		PositionTileWarning(warning, tile);
		return warning;
	}

	void PositionTileWarning(GameObject warning, Tile tile)
	{
		Camera camera = Camera.main;
		warning.transform.position = camera.WorldToScreenPoint(tile.transform.position) - new Vector3(0, 25, 0);
		float scale = referenceOrthographicSize / camera.orthographicSize;
		warning.transform.localScale = new Vector3(scale, scale, 1f);
	}

	void DestroyWarning(ref GameObject warning, ref Tile tile)
	{
		if (warning != null)
		{
			Destroy(warning);
			warning = null;
		}
		tile = null;
	}

	public void ShowCharacterWarning(Character c, string text)
	{
		HideCharacterWarning();
		characterWarningTile = TileGrid.Instance.FindCharacter(c)[0];
		characterWarning = CreateTileWarning(characterWarningTile, text);
	}

	public void HideCharacterWarning()
	{
		DestroyWarning(ref characterWarning, ref characterWarningTile);
	}

	public void ShowProvokeWarning(Tile tile)
	{
		provokeWarningTile = tile;
		provokeWarning = CreateTileWarning(tile, "x");
	}

	public void HideProvokeWarning()
	{
		DestroyWarning(ref provokeWarning, ref provokeWarningTile);
	}

	public void ShowMousedOverWarning(Tile tile, string text)
	{
		mousedOverWarningTile = tile;
		mousedOverWarning = CreateTileWarning(tile, text);
	}

	public void HideMousedOverWarning()
	{
		DestroyWarning(ref mousedOverWarning, ref mousedOverWarningTile);
	}

	void PositionWarnings()
	{
		if (mousedOverWarning != null && mousedOverWarningTile != null)
		{
			PositionTileWarning(mousedOverWarning, mousedOverWarningTile);
		}
		if (provokeWarning != null && provokeWarningTile != null)
		{
			PositionTileWarning(provokeWarning, provokeWarningTile);
		}
		if (characterWarning != null && characterWarningTile != null)
		{
			PositionTileWarning(characterWarning, characterWarningTile);
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
		c.ResetActions();
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
	}

	static public void ReturnControlToPlayer()
	{
		ActionController.Instance.ReturnFromReaction();
		HeroDisplayRouter.Instance.mainDisplay.lastCharacter = TurnControl.Instance.lastCharacter;
		HeroDisplayRouter.Instance.mainDisplay.UpdateWithLastCharacter();
	}

	static public void RemoveControlFromPlayer(Character character)
	{
		playerHasControl = false;
		storedCharacterForControl = character;
		HeroDisplayRouter.Instance.Hide(true);
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

	public void CardFinished()
	{
		Invoke("CardFinishedActual", .5f);
	}
	public void CardFinishedActual()
	{
		playerHasControl = true;
		TurnControl.Instance.Pump();
	}
}
