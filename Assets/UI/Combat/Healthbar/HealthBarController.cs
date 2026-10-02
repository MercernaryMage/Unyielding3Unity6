using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
	public enum HealthBarType
	{
		health,
		healthArmor,
		healthEnergy,
		healthReaction,
		healthArmorEnergy,
		healthReactionArmor
	}

	public List<HealthBar> healthBars;
	Character character;

	public void Set(Character c)
	{
		character = c;
	}

	public void Update()
	{
		HealthBarType healthBarType = HealthBarType.health;
		if (character.hero)
		{
			if (character.maxArmor > 0)
			{
				healthBarType = HealthBarType.healthArmorEnergy;
			}
			else
			{
				healthBarType = HealthBarType.healthEnergy;
			}
		}
		else
		{
			if (character.characterDefinition.maxThreshold > 0 && character.maxArmor > 0)
			{
				healthBarType = HealthBarType.healthReactionArmor;
			}
			else if (character.characterDefinition.maxThreshold > 0)
			{
				healthBarType = HealthBarType.healthReaction;
			}
			else if (character.maxArmor > 0)
			{
				healthBarType = HealthBarType.healthArmor;
			}
		}
		SetType(healthBarType);


		float t = character.currentHP / (float)character.maxHP;
		SetHealth(t);

		if (character.maxArmor > 0)
		{
			SetArmor(character.armor / (float)character.maxArmor);
		}
		if (character.characterDefinition.maxThreshold > 0)
		{
			SetReaction(character.threshold / (float)character.characterDefinition.maxThreshold);
		}
		if (character.hero)
		{
			SetReaction(character.currentEnergy / (float)character.characterDefinition.maxEnergy);
		}
	}

	public void SetType(HealthBarType type)
	{
		for (int i = 0; i < healthBars.Count; ++i)
		{
			healthBars[i].gameObject.SetActive(i == (int)type);
		}
	}

	public void SetHealth(float percent)
	{
		foreach (HealthBar healthBar in healthBars)
		{
			healthBar.SetHealth(percent);
		}
	}

	public void SetArmor(float percent)
	{
		foreach (HealthBar healthBar in healthBars)
		{
			healthBar.SetArmor(percent);
		}
	}

	public void SetEnergy(float percent)
	{
		foreach (HealthBar healthBar in healthBars)
		{
			healthBar.SetEnergy(percent);
		}
	}

	public void SetReaction(float percent)
	{
		foreach (HealthBar healthBar in healthBars)
		{
			healthBar.SetReaction(percent);
		}
	}
}
