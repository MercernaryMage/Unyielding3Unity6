using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFollower : MonoBehaviour
{
    List<Tile> path;

	Character character;
	Action callback;

    float time;
    float timeMax = .5f;

	bool moveCharacter;

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
	}

	public void Update()
	{
		time += Time.deltaTime;
		if (time >= timeMax)
		{
			time = 0;
			Vector3 offset = new Vector3(character.characterDefinition.size - 1,
						0,
						character.characterDefinition.size - 1);
			//transform.position = path[1].transform.position + offset;

			GlobalPositionLerp positionLerp = gameObject.AddComponent<GlobalPositionLerp>();
			positionLerp.p0 = path[0].transform.position + offset;
			positionLerp.p1 = path[1].transform.position + offset;
			positionLerp.type = InterpolationType.Smoother;
			positionLerp.runTime = .3f;
			positionLerp.Init();

			character.SetFacing(TileGrid.Instance.GetFacingDirection(path[0], path[1]));
			path.RemoveAt(0);
			if (path.Count == 1)
			{
				positionLerp.callbackFunction = () =>
				{
					if (moveCharacter)
					{
						TileGrid.Instance.MoveCharacterToTile(character, path[0]);
					}
					callback();
				};
				Destroy(this);
			}
		}
	}
}
