using UnityEditor;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform health;
	public RectTransform armor;
	public RectTransform reaction;
	public RectTransform energy;

	public float barWidth = 117;



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
}
