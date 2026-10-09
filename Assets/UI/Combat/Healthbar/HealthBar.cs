using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform health;
	public RectTransform armor;
	public RectTransform reaction;
	public RectTransform energy;

	public RectTransform statusEffects;

	public float barWidth = 117;

	Dictionary<StatusEffect, GameObject> createdEffects = new Dictionary<StatusEffect, GameObject>();

	public void SetHealth(float percent)
	{
		health.sizeDelta = new Vector2(barWidth * percent, health.sizeDelta.y);
	}

	public void SetArmor(float percent)
	{
		if (armor != null)
		{
			armor.sizeDelta = new Vector2(barWidth * percent, armor.sizeDelta.y);
		}
	}

	public void SetReaction(float percent)
	{
		if (reaction != null)
		{
			reaction.sizeDelta = new Vector2(barWidth * percent, reaction.sizeDelta.y);
		}
	}

	public void SetEnergy(float percent)
	{
		if (energy != null)
		{
			energy.sizeDelta = new Vector2(barWidth * percent, energy.sizeDelta.y);
		}
	}

	public void AddEffect(StatusEffect effect, GameObject iconObject)
	{
		createdEffects[effect] = iconObject;
		iconObject.transform.SetParent(statusEffects);
	}

	public void RemoveEffect(StatusEffect effect)
	{
		Destroy(createdEffects[effect]);
		createdEffects.Remove(effect);
	}
}
