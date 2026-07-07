
using System.Collections.Generic;
using UnityEngine;

public class DebugSwing : MonoBehaviour
{
	public Character c;
	public Character d;
	public EffectScriptableObject effect;

	private void Start()
	{
		Invoke("Setup", 1);
	}

	public void Setup()
	{
		c = BattleController.Instance.heroes[0];
		d = BattleController.Instance.enemies[0];
		foreach (Card card in d.cards)
		{
			if (card.GetCardName() == "Crimson Claw")
			{
				effect = card.cardScriptableObject.effects[0];
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.M))
		{
			List<Character> targets = new List<Character>() {d };
			ActionController.Instance.PlayAdvancedAttackAnimation(c, targets, effect, null, () => 
			{
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(d, "5");
			}
			);
		}
		if (Input.GetKeyUp(KeyCode.T)  && Input.GetKey(KeyCode.LeftShift))
		{
			Time.timeScale = .1f;
		}

	}
	

}
