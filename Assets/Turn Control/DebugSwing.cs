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
			ActionController.Instance.PlayAdvancedAttackAnimation(c, effect, () => 
			{
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(d, "5");
			}
			);
		}

	}
	

}
