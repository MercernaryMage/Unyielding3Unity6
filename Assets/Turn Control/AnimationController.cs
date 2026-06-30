using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : SceneSingleton<AnimationController>
{
    Action storedAction;

    public void ShowTiles(List<Tile> tiles, Tile.OverlayType overlayType, Action callback, Action noMovementCallback = null, float delay = 1)
	{
		if (tiles != null)
		{
			foreach (Tile t in tiles)
			{
				t.ShowOverlay(overlayType);
			}
		}
		if (noMovementCallback != null && (tiles == null || tiles.Count <= 1))
		{
			noMovementCallback();
			return;
		}
		else
		{
			storedAction = callback;
		}
		Invoke(nameof(DoCallback), delay);
	}

	public void DoCallback()
	{
		storedAction.Invoke();
	}

	public void DelayedCallback(float seconds, Action callback)
	{
		storedAction = callback;
		Invoke(nameof(DoCallback), seconds);
	}

	static public void PlayEffect(Character character, EffectScriptableObject effect, float actionDelay, Action callback)
	{
		PlayEffectOnDelay effectDelay = character.token.gameObject.AddComponent<PlayEffectOnDelay>();
		Vector3 position;

		if (effect.position == EffectScriptableObject.EffectPosition.bone)
		{
			position = character.token.GetBonePosition(effect.bone);
		}
		else
		{
			position = Vector3.zero;
		}
		effectDelay.Create(effect, 0, position, 1, 
			character.token.transform.localRotation, callback, actionDelay);
	}
}
