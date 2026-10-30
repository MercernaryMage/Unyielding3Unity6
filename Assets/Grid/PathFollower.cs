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

	public void Set(Character c, List<Tile> p, Action call, bool m)
	{
		path = p;
		character = c;
		callback = call;
		moveCharacter = m;
		if (p.Count < 2)
		{
			callback();
			Destroy(this);
			return;
		}
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
		Vector3 offset = new Vector3(character.characterDefinition.size - 1,
					0,
					character.characterDefinition.size - 1);

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
				Destroy(this);
				callback();
			};
			return;
		}

		positionLerp.callbackFunction = StepToNextTile;
	}
}
