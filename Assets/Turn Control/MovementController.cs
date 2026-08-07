using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.TextCore.Text;

public class MovementController : SceneSingleton<MovementController>
{
	Tile currentMousedOverTile = null;
	public List<Tile> currentPossibleTiles = new List<Tile>();
	public bool running = false;

	public Character movingCharacter;

	public Tile previousMousedTile;

	bool provoked;
	Tile damageWarningTile;
	string damageWarningText;

	public void HideMovement()
	{
		foreach (Tile tile in currentPossibleTiles)
		{
			tile.HideOverlay(Tile.OverlayType.PossibleMovement);
			tile.HideOverlay(Tile.OverlayType.Selected);
		}
		currentPossibleTiles.Clear();
		currentMousedOverTile = null;
		running = false;
		movingCharacter = null;

		provoked = false;
		damageWarningTile = null;

		BattleController.Instance.HideWarnings();
	}

	void RefreshWarnings()
	{
		BattleController.Instance.HideWarnings();

		if (provoked && movingCharacter != null)
		{
			BattleController.Instance.ShowWarning(TileGrid.Instance.FindCharacter(movingCharacter)[0], "x");
		}

		if (damageWarningTile != null)
		{
			BattleController.Instance.ShowWarning(damageWarningTile, damageWarningText);
		}
	}

	public Action onMoveComplete;

    public void ShowMovement(Character character, int rangeLimit = -1)
	{
		HideMovement();
		running = true;
		movingCharacter = character;
		List<Tile> tiles = TileGrid.Instance.FindCharacter(character);

		int range = rangeLimit >= 0 ? Mathf.Min(character.currentMovement, rangeLimit) : character.currentMovement;
		List<Tile> inRangeTiles = GetAllTilesInRange(character, range);

		foreach (Tile t in inRangeTiles)
		{
			if (t.character == null)
			{
				t.ShowOverlay(Tile.OverlayType.PossibleMovement);
				currentPossibleTiles.Add(t);
			}
		}

		if (previousMousedTile != null && currentPossibleTiles.Contains(previousMousedTile))
		{
			currentMousedOverTile = previousMousedTile;
			currentMousedOverTile.HideOverlay(Tile.OverlayType.PossibleMovement);
			currentMousedOverTile.ShowOverlay(Tile.OverlayType.Selected);
		}

		PreviewMovementProvokeMessage provokeMessage = new PreviewMovementProvokeMessage();
		provokeMessage.movingCharacter = character;
		MessagePump.Instance.SendMessage(provokeMessage);

		provoked = provokeMessage.provoked;
		RefreshWarnings();
	}

	public List<Tile> GetAllTilesInRange(Character c, int range)
	{
		List<Tile> inRangeTiles = new List<Tile>();

		List<Tile> characterTiles = TileGrid.Instance.FindCharacter(c);

		for (int j = 0; j < characterTiles.Count; ++j)
		{
			for (int i = 0; i < TileGrid.Instance.tiles.Count; ++i)
			{
				if (inRangeTiles.Contains(TileGrid.Instance.tiles[i]))
				{
					continue;
				}
				MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
				rules.allowedToPathThroughAllies = true;
				List<Tile> route = FindRoute(c, TileGrid.Instance.tiles[i], j, rules);
				if (route != null && route.Count <= range + 1)
				{
					inRangeTiles.Add(TileGrid.Instance.tiles[i]);
				}
			}
		}
		return inRangeTiles;
	}

	public void MouseOverTile(Tile t)
	{
		if (currentMousedOverTile)
		{
			currentMousedOverTile.HideOverlay(Tile.OverlayType.Selected);
			currentMousedOverTile.ShowOverlay(Tile.OverlayType.PossibleMovement);
			damageWarningTile = null;
			RefreshWarnings();
			currentMousedOverTile = null;
		}
		if (!currentPossibleTiles.Contains(t))
		{
			return;
		}
		currentMousedOverTile = t;
		currentMousedOverTile.ShowOverlay(Tile.OverlayType.Selected);
		currentMousedOverTile.HideOverlay(Tile.OverlayType.PossibleMovement);

		PathfindingRules rules = new PathfindingRules();
		rules.allowedToPathThroughAllies = true;
		List<Tile> route = FindRoute(movingCharacter, t, 0, rules);
		int tilesMoved = route != null ? route.Count - 1 : 0;

		PreviewMovementDamageMessage previewMessage = new PreviewMovementDamageMessage();
		previewMessage.movingCharacter = movingCharacter;
		previewMessage.tilesMoved = tilesMoved;
		MessagePump.Instance.SendMessage(previewMessage);

		if (previewMessage.damage > 0)
		{
			damageWarningTile = t;
			damageWarningText = previewMessage.damage.ToString();
			RefreshWarnings();
		}
	}

	public void MouseExitTile(Tile t)
	{
		if (running)
		{
			if (currentMousedOverTile == t)
			{
				currentMousedOverTile.HideOverlay(Tile.OverlayType.Selected);
				currentMousedOverTile.ShowOverlay(Tile.OverlayType.PossibleMovement);
				damageWarningTile = null;
				RefreshWarnings();
				currentMousedOverTile = null;
			}
		}
	}

	public bool HandleClick()
	{
		if (currentMousedOverTile)
		{
			MoveCharacter(currentMousedOverTile);
			HideMovement();
			return true;
		}
		return false;
	}

	public void MoveCharacterLikeAI(Character c, Tile t, Action movementCompleteCallback = null)
	{
		movingCharacter = c;
		MoveCharacter(t, movementCompleteCallback);
	}

	//player moving
	public void MoveCharacter(Tile t, Action movementCompleteCallback = null)
	{
		movingCharacter.canUseCumbersome = false;
		CharacterStartMovementMessage characterStartMovementMessage = new CharacterStartMovementMessage();
		characterStartMovementMessage.movingCharacter = movingCharacter;
		characterStartMovementMessage.provokeTriggers = onMoveComplete == null;
		MessagePump.Instance.SendMessage(characterStartMovementMessage);

		BattleController.playerHasControl = false;

		Character oldMovingCharacter = movingCharacter;
		Action stepCallback = onMoveComplete;
		onMoveComplete = null;

		RunTriggersThenMove(characterStartMovementMessage.raisedTriggers, 0, oldMovingCharacter, t, stepCallback, movementCompleteCallback);
	}

	void RunTriggersThenMove(List<Trigger> triggers, int index, Character oldMovingCharacter, Tile t, Action stepCallback, Action movementCompleteCallback)
	{
		if (index < triggers.Count && oldMovingCharacter.alive)
		{
			triggers[index].onComplete = () =>
			{
				RunTriggersThenMove(triggers, index + 1, oldMovingCharacter, t, stepCallback, movementCompleteCallback);
			};
			triggers[index].Click();
			return;
		}

		if (!oldMovingCharacter.alive)
		{
			BattleController.playerHasControl = true;
			if (movementCompleteCallback != null)
			{
				movementCompleteCallback();
			}
			FinishMovement(stepCallback);
			return;
		}

		PathfindingRules pathfindingRules = new PathfindingRules();
		pathfindingRules.allowedToPathThroughAllies = true;
		List<Tile> route = FindRoute(oldMovingCharacter, t, 0, pathfindingRules);
		//oldMovingCharacter.currentMovement -= (route.Count - 1);
		oldMovingCharacter.currentMovement = 0;
		PathFollower pathFollower = oldMovingCharacter.token.gameObject.AddComponent<PathFollower>();
		pathFollower.Set(oldMovingCharacter, route, () =>
		{
			Character closestEnemy = TileGrid.Instance.FindClosestEnemy(oldMovingCharacter);
			if (closestEnemy)
			{
				Direction direction = TileGrid.Instance.GetFacingDirection(oldMovingCharacter, closestEnemy);
				oldMovingCharacter.SetFacing(direction);
			}

			CharacterFinishedMovingMessage message = new CharacterFinishedMovingMessage(() =>
			{
				FinishMovement(stepCallback);
			});
			message.movingCharacter = oldMovingCharacter;
			MessagePump.Instance.SendMessage(message);

			if (message.waitingObjects.Count > 0)
			{
				return;
			}
			if (movementCompleteCallback != null)
			{
				movementCompleteCallback();
			}
			FinishMovement(stepCallback);
		}, true);
	}

	void FinishMovement(Action stepCallback)
	{
		BattleController.playerHasControl = true;
		UIController.Instance.UpdateAfterUsage();

		stepCallback?.Invoke();
	}

	public class PathfindingRules
	{
		public bool allowedToPathThroughAllies;
	}


	public List<Tile> FindRoute(Character c, Tile destination, int offsetIndex, PathfindingRules rules)
	{
		List<Tile> startingTiles = TileGrid.Instance.FindCharacter(c);
		Tile startingTile = startingTiles[0];
		List<Tuple<List<Tile>,int>> openList = new List<Tuple<List<Tile>, int>>();
		List<Tile> closestList = new List<Tile>();


		Tuple<List<Tile>, int> firstNode = new Tuple<List<Tile>, int>(new List<Tile>() {startingTile },0);

		openList.Add(firstNode);
		while(openList.Count > 0)
		{
			List<Tile> currentTileRoute = openList[0].Item1;
			openList.RemoveAt(0);
			if (currentTileRoute[0] == destination)
			{
				currentTileRoute.Reverse();
				return currentTileRoute;
			}
			List<Tile> adjacentTiles = TileGrid.Instance.GetAdjacentTiles(currentTileRoute[0]);
			foreach (Tile adjacentTile in adjacentTiles)
			{
				if (closestList.Contains(adjacentTile))
				{
					continue;
				}
				Tile trueTile = TileGrid.Instance.GetTrueTileFromOffset(adjacentTile, offsetIndex, c.characterDefinition.size);
				if (trueTile == null)
				{
					continue;
				}
				List<Tile> fitTilesAtLocation = TileGrid.Instance.WhatTilesWouldCharacterTake(c, trueTile);
				bool fail = false;
				if (fitTilesAtLocation == null)
				{
					continue;
				}
				foreach (Tile fitTileAtLocation in fitTilesAtLocation)
				{
					if (fitTileAtLocation == null ||
						!fitTileAtLocation.tileScriptableObject.enterable ||
						(fitTileAtLocation.character && 
						fitTileAtLocation.character.hero != c.hero) ||
						(fitTileAtLocation.character && !rules.allowedToPathThroughAllies))
					{
						fail = true;
						break;
					}
				}
				if (fail)
				{
					continue;
				}

				closestList.Add(adjacentTile);

				List<Tile> route = new List<Tile>(currentTileRoute);
				route.Insert(0, adjacentTile);
				int distance = TileGrid.Distance(adjacentTile, destination);
				
				openList.Add(new Tuple<List<Tile>, int>(route, distance));
			}
			openList = openList.OrderBy(o => o.Item2).ToList();
		}

		return null;
	}

}
