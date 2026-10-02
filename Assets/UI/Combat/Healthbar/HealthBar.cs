using UnityEditor;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform health;
	public RectTransform armor;
	public RectTransform reaction;
	public RectTransform energy;

	float healthWidth;
	float armorWidth;
	float reactionWidth;
	float energyWidth;

	private void Awake()
	{
		healthWidth = health.sizeDelta.x;
		if (armor != null)
		{
			armorWidth = armor.sizeDelta.x;
		}
		if (reaction != null)
		{
			reactionWidth = reaction.sizeDelta.x;
		}
		if (energy != null)
		{
			energyWidth = energy.sizeDelta.x;
		}
	}

	public void SetHealth(float percent)
	{
		health.sizeDelta = new Vector2(healthWidth * percent, health.sizeDelta.y);
	}

	public void SetArmor(float percent)
	{
		if (armor != null)
		{
			armor.sizeDelta = new Vector2(armorWidth * percent, armor.sizeDelta.y);
		}
	}

	public void SetReaction(float percent)
	{
		if (reaction != null)
		{
			reaction.sizeDelta = new Vector2(reactionWidth * percent, reaction.sizeDelta.y);
		}
	}

	public void SetEnergy(float percent)
	{
		if (energy != null)
		{
			energy.sizeDelta = new Vector2(energyWidth * percent, energy.sizeDelta.y);
		}
	}
}
