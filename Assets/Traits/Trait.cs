using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Trait : MonoBehaviour, IMessageReceiver
{
	public Character character;
	public TraitScriptableObject scriptableObject;

	virtual public void Start()
	{
		character = GetComponent<Character>();
		MessagePump.Instance.AddListener(this);
	}

	public void OnDestroy()
	{
		//this stupid line is to prevent the unity singleton shutdown errors
		if (MessagePump.Instance)
		{
			MessagePump.Instance.RemoveListener(this);
		}
	}

	virtual public void CharacterStartMoving(CharacterStartMovementMessage message) { }
	virtual public void CharacterStartTurn(CharacterStartTurnMessage message) { }
	virtual public void CharacterEndTurn(CharacterEndTurnMessage message) { }
	virtual public void AttackComplete(AttackCompleteMessage message) { }
	virtual public void CharacterAttack(CharacterAttackingMessage message) { }
	virtual public void PreDamageDealt(PreDamageDealtMessage message) { }
	virtual public void DamageDealt(DamageDealtMessage message) { }
	virtual public void CardStart(CardStartMessage message) { }

	virtual public void CombatStart(CombatStartMessage message) { }

	public void ReceiveMessage(Message message)
	{
		if (message.messageType == MessageType.CharacterMovementStarted)
		{
			CharacterStartMoving((CharacterStartMovementMessage)message);
		}
		else if (message.messageType == MessageType.CharacterStartTurn)
		{
			CharacterStartTurn((CharacterStartTurnMessage)message);
		}
		else if (message.messageType == MessageType.CharcterEndTurn)
		{
			CharacterEndTurn((CharacterEndTurnMessage)message);
		}
		else if (message.messageType == MessageType.AttackComplete)
		{
			AttackComplete((AttackCompleteMessage)message);
		}
		else if (message.messageType == MessageType.CharacterAttacking)
		{
			CharacterAttack((CharacterAttackingMessage)message);
		}
		else if (message.messageType == MessageType.PreDamageDealt)
		{
			PreDamageDealt((PreDamageDealtMessage)message);
		}
		else if (message.messageType == MessageType.DamageDealt)
		{
			DamageDealt((DamageDealtMessage)message);
		}
		else if (message.messageType == MessageType.CardStart)
		{
			CardStart((CardStartMessage)message);
		}
		else if (message.messageType == MessageType.CombatStart)
		{
			CombatStart((CombatStartMessage)message);
		}
	}
}

public interface GuestSetter
{
	public void SetGuest(Guest guest);
}
