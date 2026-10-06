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
	public GameObject statusEffectIconPrefab;
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
			SetEnergy(character.currentEnergy / (float)character.characterDefinition.maxEnergy);
		}
	}

	public void SetType(HealthBarType type)
	{
		for (int i = 0; i < healthBars.Count; ++i)
		{
			healthBars[i].gameObject.SetActive(i == (int)type);
		}
	}

	public Vector3 GetWorldPosition()
	{
		Camera camera = Camera.main;
		if (camera == null || character == null || character.token == null)
		{
			return transform.position;
		}

		Transform barTransform = transform;
		foreach (HealthBar healthBar in healthBars)
		{
			if (healthBar.gameObject.activeSelf)
			{
				barTransform = healthBar.transform;
				break;
			}
		}

		Vector3 screenPosition = barTransform.position;
		screenPosition.z = camera.WorldToScreenPoint(character.token.transform.position).z;
		return camera.ScreenToWorldPoint(screenPosition);
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

	public void AddEffectIcon(Sprite s)
	{

	}
}
