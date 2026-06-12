using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class StatusEffect : MonoBehaviour, IMessageReceiver
{
	public Character character;

	bool exiting = false;

	virtual public void Start()
	{
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnEditorChangeState;
#endif

		character = GetComponent<Character>();
		MessagePump.Instance.AddListener(this);

		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, GetDisplayName());
	}

#if UNITY_EDITOR
	public void OnEditorChangeState(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.ExitingPlayMode)
		{
			exiting = true;
		}
	}
#endif

	public virtual void EffectBeingRemoved()
	{

	}

	public void OnDestroy()
	{
		if (exiting)
		{
			return;
		}
		//this stupid line is to prevent the unity singleton shutdown errors
		if (MessagePump.Instance)
		{
			EffectBeingRemoved();
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, $"-{GetDisplayName()}");
			MessagePump.Instance.RemoveListener(this);
			this.enabled = false;

			FancyHeroDisplay display = HeroDisplayRouter.Instance.mainDisplay;
			if (display.showing && display.lastCharacter == character)
			{
				display.statusEffectDisplayGroup.Set(character);
			}
		}
	}
	virtual public void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage) { }
	virtual public void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage) { }
	virtual public void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage) { }
	virtual public void OnCharacterMiss(CharacterMissMessage characterMissMessage) { }

	virtual public void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage) { }
	virtual public void OnHeroDowned(HeroDownedMessage heroDownedMessage) { }

	virtual public void OnDamageDealt(DamageDealtMessage damageDealtMessage) { }
	virtual public void OnCharacterCritCheck(CharacterCritCheckMessage characterCritCheckMessage) { }
	virtual public void OnCharacterCrit(CharacterCritMessage characterCritMessage) { }
	virtual public void OnAttackComplete(AttackCompleteMessage attackCompleteMessage) { }
	virtual public void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage) { }
	virtual public void OnCharacterTakeBurnDamage(CharacterTakingBurnDamageMessage characterTakingBurnDamageMessage) { }

	public void ReceiveMessage(Message message)
	{
		if (message.messageType == MessageType.CharacterStartTurn)
		{
			CharacterStartTurn((CharacterStartTurnMessage)message);
		}
		else if (message.messageType == MessageType.CharcterEndTurn)
		{
			if (character.gameObject.GetComponent<Stasis>() != null && GetType() != typeof(Stasis))
			{
				return;
			}
			CharacterEndTurn((CharacterEndTurnMessage)message);
		}
		else if (message.messageType == MessageType.CharacterAttacking)
		{
			OnCharacterAttacking((CharacterAttackingMessage)message);
		}
		else if (message.messageType == MessageType.CharacterMiss)
		{
			OnCharacterMiss((CharacterMissMessage)message);
		}
		else if (message.messageType == MessageType.DamageDealt)
		{
			OnDamageDealt((DamageDealtMessage)message);
		}
		else if (message.messageType == MessageType.HeroDowned)
		{
			OnHeroDowned((HeroDownedMessage)message);
		}
		else if (message.messageType == MessageType.CharacterFinishedMoving)
		{
			OnCharacterFinishedMoving((CharacterFinishedMovingMessage)message);
		}
		else if (message.messageType == MessageType.CharacterCritCheck)
		{
			OnCharacterCritCheck((CharacterCritCheckMessage)message);
		}
		else if (message.messageType == MessageType.CharacterCrit)
		{
			OnCharacterCrit((CharacterCritMessage)message);
		}
		else if (message.messageType == MessageType.AttackComplete)
		{
			OnAttackComplete((AttackCompleteMessage)message);
		}
		else if (message.messageType == MessageType.PreDamageDealt)
		{
			OnPreDamageDealt((PreDamageDealtMessage)message);
		}
		else if (message.messageType == MessageType.CharacterTakingBurnDamage)
		{
			OnCharacterTakeBurnDamage((CharacterTakingBurnDamageMessage)message); ;
		}
	}

	public virtual string GetIconName()
	{
		return "";
	}

	public abstract string GetDisplayName();
	public abstract string GetEffectText();

	public class StatusEffectInitData
	{
		public int magnitude;
	}

	public virtual void DoStack(StatusEffectInitData data)
	{

	}
}
