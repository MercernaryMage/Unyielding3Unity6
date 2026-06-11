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

	Character movingCharacter;

	public Tile previousMousedTile;

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
	}

	List<Tile> GetAllTilesInRange(Character c, int range)
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
				if (route != null && route.Count <= range)
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
			currentMousedOverTile = null;
		}
		if (!currentPossibleTiles.Contains(t))
		{
			return;
		}	
		currentMousedOverTile = t;
		currentMousedOverTile.ShowOverlay(Tile.OverlayType.Selected);
		currentMousedOverTile.HideOverlay(Tile.OverlayType.PossibleMovement);
	}

	public void MouseExitTile(Tile t)
	{
		if (running)
		{
			if (currentMousedOverTile == t)
			{
				currentMousedOverTile.HideOverlay(Tile.OverlayType.Selected);
				currentMousedOverTile.ShowOverlay(Tile.OverlayType.PossibleMovement);
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

	//player moving
	public void MoveCharacter(Tile t)
	{
		CharacterStartMovementMessage characterStartMovementMessage = new CharacterStartMovementMessage();
		characterStartMovementMessage.movingCharacter = movingCharacter;
		characterStartMovementMessage.provokeTriggers = onMoveComplete == null;
		MessagePump.Instance.SendMessage(characterStartMovementMessage);

		foreach (Trigger trigger in characterStartMovementMessage.raisedTriggers)
		{
			trigger.Click();
		}

		BattleController.playerHasControl = false;
		
		List<Tile> exitingTiles = TileGrid.Instance.FindCharacter(movingCharacter);
		PathfindingRules pathfindingRules = new PathfindingRules();
		pathfindingRules.allowedToPathThroughAllies = true;
		List<Tile> route = FindRoute(movingCharacter, t, 0, pathfindingRules);
		movingCharacter.currentMovement -= (route.Count - 1);
		PathFollower pathFollower = movingCharacter.token.gameObject.AddComponent<PathFollower>();
		Character oldMovingCharacter = movingCharacter;
		Action stepCallback = onMoveComplete;
		onMoveComplete = null;
		pathFollower.Set(movingCharacter, route, () =>
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
