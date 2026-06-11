using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.TextCore.Text;
using static StatusEffect;

public class Character : MonoBehaviour
{
	public string displayName;
	public int displayNumber;
	public CharacterToken token;
	public CharacterScriptableObject characterDefinition;
	public StorageCharacter storageCharacter;
	public int movementMax;
	public int currentMovement;
	public bool hero;
	public int currentHP;
	public int currentEnergy;
	public int maxHP;
	public int maxArmor;
	public int currentEvasion;
	public bool alive = true;

	public int threshold = 0;

	public int actionCount = 2;

	public int armor;
	public int toughness;

	public int triggerCount = 1;

	public List<Item> temporaryItems = new List<Item>();

	public List<Card> cards = new List<Card>();
	public List<Card> cardDiscard = new List<Card>();
	public List<Reaction> reactions = new List<Reaction>();
	public List<Reaction> reactionDiscard = new List<Reaction>();

	public Direction facing;

	public void Init(CharacterScriptableObject characterScriptableObject)
	{
		characterDefinition = characterScriptableObject;
		displayName = characterDefinition.displayName;
		GameObject tokenObject = Instantiate(characterDefinition.model);
		token = tokenObject.AddComponent<CharacterToken>();
		token.owningCharacter = this;
		tokenObject.transform.localScale = Vector3.one * characterDefinition.size;
		movementMax = characterScriptableObject.movement;
		currentHP = characterDefinition.maxHP;
		currentEnergy = characterDefinition.maxEnergy;
		maxHP = characterDefinition.maxHP;
		armor = characterDefinition.armor;
		maxArmor = characterDefinition.armor;
		toughness = characterDefinition.toughness;
		currentEvasion = characterDefinition.evasion;

		GameObject healthBar = Instantiate(CharacterRepository.Instance.data.healthBarPrefab);
		healthBar.transform.SetParent(tokenObject.transform);
		healthBar.GetComponent<CharacterHealthBar>().Set(this);
		if (characterDefinition.size == 1)
		{
			healthBar.transform.localPosition = new Vector3(0, -1.3f, 0);
		}
		else if (characterDefinition.size == 2)
		{
			healthBar.transform.localPosition = new Vector3(0, .29f, 0);
		}

		foreach (CardScriptableObject cardScriptableObject in characterDefinition.cards)
		{
			Type t = Type.GetType(cardScriptableObject.className);
			Card c = (Card)Activator.CreateInstance(t);
			c.Set(cardScriptableObject);
			c.owningCharacter = this;
			cards.Add(c);
		}
		foreach (ReactionScriptableObject reactionScriptableObject in characterDefinition.reactions)
		{
			Reaction reaction = new Reaction();

			{
				Type t = Type.GetType(reactionScriptableObject.normalReaction.className);
				ReactionBase c = (ReactionBase)Activator.CreateInstance(t);
				c.Set(reactionScriptableObject.normalReaction);
				c.owningCharacter = this;
				reaction.normalReaction = c;
			}
			{
				if (reactionScriptableObject.aggravatedReaction)
				{
					Type t = Type.GetType(reactionScriptableObject.aggravatedReaction.className);
					ReactionBase c = (ReactionBase)Activator.CreateInstance(t);
					c.Set(reactionScriptableObject.aggravatedReaction);
					c.owningCharacter = this;
					reaction.aggravatedReaction = c;
				}
			}
			reactions.Add(reaction);
		}
		Util.Shuffle(cards);
		Util.Shuffle(reactions);
		BattleController.Instance.AddCharacter(this);
	}

	public void ResetActions()
	{
		if (storageCharacter != null)
		{
			foreach (Item i in storageCharacter.equipment)
			{
				i.Reset();
			}
			foreach (Item i in temporaryItems)
			{
				i.Reset();
			}
		}
	}

	public void SetFacing(Direction facing)
	{
		this.facing = facing;
		token.transform.localRotation = Util.GetFacing(facing);
	}

	public Component AddStatusEffect(Type statusEffectType, StatusEffectInitData data)
	{
		Component component = GetComponent(statusEffectType);
		if (component == null)
		{
			component = gameObject.AddComponent(statusEffectType);
		}
		StatusEffect effect = (StatusEffect)component;
		effect.DoStack(data);
		return component;
	}

	public void RefillArmor()
	{
		int armorDifference = maxArmor - armor;
		if (armorDifference > 0)
		{
			maxArmor -= Mathf.Max(armorDifference / 2, 1);
		}
		
		armor = maxArmor;
	}

	public void Downed(int overflow)
	{
		HeroDownedMessage heroDownedMessage = new HeroDownedMessage();
		heroDownedMessage.downedCharacter = this;
		MessagePump.Instance.SendMessage(heroDownedMessage);
		Downed downed = (Downed)AddStatusEffect(typeof(Downed), null);
		downed.overflow += overflow;
	}

	public int GetInitiative()
	{
		return Util.RollDice(1, 20);
	}

	public bool ShouldHaveHalfMovement()
	{
		if (gameObject.GetComponent<Exhausted>() != null ||
			gameObject.GetComponent<Defend>())
		{
			return true;
		}
		return false;
	}

	public void AddMovement()
	{
		currentMovement = movementMax;
		if (ShouldHaveHalfMovement())
		{
			currentMovement = Mathf.Max(1, currentMovement / 2);
		}
	}

	public void RestoreAllEnergy()
	{
		currentEnergy = characterDefinition.maxEnergy;
	}


	public void StartTurn()
	{
		AddMovement();
		actionCount = 4;
		if (ShouldHaveHalfMovement())
		{
			actionCount = Mathf.Max(1, actionCount / 2);
		}
		triggerCount = 1;
		RefillArmor();
		ResetActions();
	}

	public void Die()
	{
		alive = false;
		Trait[] traits = GetComponents<Trait>();
		foreach (Trait t in traits)
		{
			Destroy(t);
		}
	}

	public void SpendEnergy(int cost)
	{
		if (cost == 0)
		{
			return;
		}
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(this, $"-{cost} energy");

		currentEnergy -= cost;
		++storageCharacter.surgeIndex;

		if (currentEnergy <= 0)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				AddStatusEffect(typeof(Vulnerable), null);
			}
			else
			{
				AddStatusEffect(typeof(Exhausted), null);
			}
		}
	}

	public void ShadeToken()
	{
		token.Shade();
	}

	public void UnshadeToken()
	{
		token.UnShade();
	}

	public bool IsDowned()
	{
		return GetComponent<Downed>() != null;
	}
}
