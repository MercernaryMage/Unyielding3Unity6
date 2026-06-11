using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UIController : SceneSingleton<UIController>
{
	public void Hide()
	{
		HeroDisplayRouter.Instance.Hide(false) ;
		EnemyDisplay.Instance.Hide();
	}

	public void NothingClicked()
	{
		if (BattleController.playerHasControl == false)
		{
			return;
		}
		if (ActionController.Instance.running)
		{
			Character attackingCharacter = ActionController.Instance.attackingCharacter;
			ActionController.Instance.HideAttack();
			MovementController.Instance.ShowMovement(attackingCharacter);
		}
		else if (ActionController.Instance.stepRunning)
		{
			ActionController.Instance.ExecuteStep();
		}
		else
		{
			Hide();
		}

	}

	public void ShowCharacter(Character character)
	{
		if (character.hero)
		{
			ShowHero(character, false);
			HideEnemy();
			return;
		}
		else
		{
			HideHero();
			ShowEnemy(character);
		}
	}

	public void ShowEnemy(Character character)
	{
		EnemyDisplay.Instance.Show();
		EnemyDisplay.Instance.Set(character);
	}

	public void HideEnemy()
	{
		EnemyDisplay.Instance.Hide();
	}

	public void ShowHero(Character character, bool main)
	{
		HeroDisplayRouter.Instance.Show(main);
		HeroDisplayRouter.Instance.Set(character, main);
	}

	public void HideHero()
	{
		HeroDisplayRouter.Instance.Hide(false);
	}

	public void UpdateAfterUsage()
	{
		if (HeroDisplayRouter.Instance.mainDisplay.showing)
		{
			HeroDisplayRouter.Instance.UpdateWithLastCharacter(true);
		}
	}
}
