using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UIController : SceneSingleton<UIController>
{
	public Transform worldUI;
	public FancyHeroDisplay heroDisplay;


	public void Hide()
	{
		heroDisplay.Hide(false) ;
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
			ShowHero(character, true);
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
		CharacterInspector.Instance.Set(character);
	}

	public void ShowHero(Character character, bool useInspector)
	{
		if (useInspector)
		{
			CharacterInspector.Instance.Set(character);
		}
		else
		{
			heroDisplay.Show();
			heroDisplay.Set(character);
		}
	}

	public void HideHero()
	{
		heroDisplay.Hide(false);
	}

	public void UpdateAfterUsage()
	{
		if (heroDisplay.showing)
		{
			heroDisplay.UpdateWithLastCharacter();
		}
	}

	public void UpdateWithLastCharacter()
	{
		heroDisplay.UpdateWithLastCharacter();
	}
}
