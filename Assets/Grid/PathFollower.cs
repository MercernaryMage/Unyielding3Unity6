using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFollower : MonoBehaviour
{
    List<Tile> path;

	Character character;
	Action callback;

	float timePerTile = .3f;

	float cameraCatchupTime;

	bool moveCharacter;
	bool finished;
	bool firstTile = true;
	bool started;
	bool resolved;

	Tile destination;
	Tile facingFromTile;

	public void Set(Character c, List<Tile> p, Action call, bool m)
	{
		path = p;
		character = c;
		callback = call;
		moveCharacter = m;
		if (p.Count < 2)
		{
			resolved = true;
			callback();
			Destroy(this);
			return;
		}
		destination = p[p.Count - 1];
		facingFromTile = p[p.Count - 2];
		cameraCatchupTime = SelectionManager.Instance.SnapCameraToCharacter(character);
	}

	InterpolationType GetInterpolationType()
	{
		if (firstTile)
		{
			firstTile = false;
			return InterpolationType.EaseOut;
		}
		if (path.Count == 2)
		{
			return InterpolationType.EaseIn;
		}
		return InterpolationType.Simple;
	}

	public void Update()
	{
		if (finished)
		{
			return;
		}

		if (cameraCatchupTime > 0)
		{
			cameraCatchupTime -= Time.deltaTime;
			return;
		}

		SelectionManager.Instance.CenterCameraOnCharacter(character);

		if (started)
		{
			return;
		}

		started = true;
		StepToNextTile();
	}

	void StepToNextTile()
	{
		Vector3 offset = GetOffset();

		GlobalPositionLerp positionLerp = gameObject.AddComponent<GlobalPositionLerp>();
		positionLerp.p0 = path[0].transform.position + offset;
		positionLerp.p1 = path[1].transform.position + offset;
		positionLerp.type = GetInterpolationType();
		positionLerp.runTime = timePerTile;
		positionLerp.Init();

		character.SetFacing(TileGrid.Instance.GetFacingDirection(path[0], path[1]));
		path.RemoveAt(0);
		if (path.Count == 1)
		{
			CameraFocusCharacter focus = character.gameObject.AddComponent<CameraFocusCharacter>();
			focus.character = character;
			finished = true;
			positionLerp.callbackFunction = () =>
			{
				Destroy(focus);
				if (moveCharacter)
				{
					TileGrid.Instance.MoveCharacterToTile(character, path[0]);
				}
				resolved = true;
				Destroy(this);
				callback();
			};
			return;
		}

		positionLerp.callbackFunction = StepToNextTile;
	}

	public void ResolveInstantly()
	{
		if (resolved)
		{
			return;
		}
		resolved = true;
		finished = true;

		GlobalPositionLerp gpl = gameObject.GetComponent<GlobalPositionLerp>();
		if (gpl)
		{
			Destroy(gpl);
		}
		CameraFocusCharacter cfc = gameObject.GetComponent<CameraFocusCharacter>();
		if (cfc)
		{
			Destroy(cfc);
		}

		TileGrid.Instance.TeleportCharacterToTile(character, facingFromTile, destination, moveCharacter);

		Action storedCallback = callback;
		Destroy(this);
		storedCallback();
	}

	Vector3 GetOffset()
	{
		return TileGrid.Instance.GetTokenOffset(character);
	}
}
