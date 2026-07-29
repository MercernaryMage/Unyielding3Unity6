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

	public void ScrollToCharacter(Character character, Action callback, float delay = 0.5f)
	{
		float scrollTime = SelectionManager.Instance.SnapCameraToCharacter(character);
		DelayedCallback(scrollTime + delay, callback);
	}

	public void ScrollToTile(Tile tile, Action callback, float delay = 0.5f)
	{
		float scrollTime = SelectionManager.Instance.SnapCameraToTile(tile);
		DelayedCallback(scrollTime + delay, callback);
	}

	static public void PlayEffect(Character character, Vector3 seconadryPosition, EffectScriptableObject effect, float actionDelay, Action callback)
	{
		PlayEffectOnDelay effectDelay = character.token.gameObject.AddComponent<PlayEffectOnDelay>();
		Vector3 position;
		Vector3 scale = Vector3.one;
		Vector3 euler = character.token.transform.localRotation.eulerAngles;
		if (effect.position == EffectScriptableObject.EffectPosition.bone)
		{
			position = character.token.GetBonePosition(effect.bone);
		}
		else if (effect.position == EffectScriptableObject.EffectPosition.halfway)
		{
			Vector3 midway = seconadryPosition - character.token.transform.position;
			position = character.token.transform.position + midway / 2;
			scale = new Vector3(1, 1, midway.magnitude * .6666f);
			float angleBetween = Vector3.SignedAngle(Vector3.right, midway, Vector3.up);
			euler = new Vector3(0, angleBetween + 90, 0);
		}
		else
		{
			position = character.token.transform.position;
		}
		effectDelay.Create(effect, effect.delay, position, scale,
			Quaternion.Euler(euler), callback, actionDelay);
	}

	static public void PlayEffect(Character character, EffectScriptableObject effect, float actionDelay, Action callback)
	{
		PlayEffect(character, Vector3.one, effect, actionDelay, callback);
	}
}
