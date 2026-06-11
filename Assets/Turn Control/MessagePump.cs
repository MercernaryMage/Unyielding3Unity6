using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessageReceiver
{
	public void ReceiveMessage(Message message);
}

public enum MessageType
{
	StartRound,
	CharacterStartTurn,
	CharcterEndTurn,
	EndRound,
	CharacterAttacking,
	CharacterMiss,
	PreDamageDealt,
	DamageDealt,
	HeroDowned,
	CharacterMovementStarted,
	CharacterCritCheck,
	CharacterCrit,
	AttackComplete,
	CharacterFinishedMoving,
	CardStart,
	CombatStart,
	CharacterTakingBurnDamage
}

public class Message
{
	public MessageType messageType;
}

public class CharacterEndTurnMessage : Message
{
	public Character character;
	public CharacterEndTurnMessage()
	{
		messageType = MessageType.CharcterEndTurn;
	}
}

public class CharacterStartTurnMessage : Message
{
	public Character character;

	public List<Object> turnStartLocks = new List<Object>();
	public CharacterStartTurnMessage()
	{
		messageType = MessageType.CharacterStartTurn;
	}
}


public class CharacterAttackingMessage : Message
{
	public Character attacker;
	public Character defender;
	public int accuracy = 0;
	public bool autoMiss = false;
	public bool autoCrit = false;
	public ActionPattern pattern;
	public string accuracyString;

	public CharacterAttackingMessage()
	{
		messageType = MessageType.CharacterAttacking;
	}

	public void AddToAccuracyString(string s)
	{
		if (!string.IsNullOrEmpty(accuracyString))
		{
			accuracyString += ", ";
		}
		accuracyString += s;
	}
}

public class CharacterMissMessage : Message
{
	public Character attacker;
	public Character defender;

	public CharacterMissMessage()
	{
		messageType = MessageType.CharacterMiss;
	}
}

public class PreDamageDealtMessage : Message
{
	public Character attacker;
	public Character defender;
	public int damage;
	public bool ranged;
	public bool hasResistance;

	public PreDamageDealtMessage()
	{
		messageType = MessageType.PreDamageDealt;
	}
}

public class DamageDealtMessage : Message
{
	public Character attacker;
	public Character defender;
	public int damage;

	public DamageDealtMessage()
	{
		messageType = MessageType.DamageDealt;
	}
}

public class AttackCompleteMessage : Message
{
	public Character attacker;
	public Character defender;
	public bool hit;
	public bool crit;
	public int damage;
	public List<Trigger> raisedTriggers = new List<Trigger>();

	public AttackCompleteMessage()
	{
		messageType = MessageType.AttackComplete;
	}
	
}

public class HeroDownedMessage : Message
{

	public Character downedCharacter;

	public HeroDownedMessage()
	{
		messageType = MessageType.HeroDowned;
	}
}

public class CharacterStartMovementMessage : Message
{
	public Character movingCharacter;
	public bool provokeTriggers;
	public List<Trigger> raisedTriggers = new List<Trigger>();

	public CharacterStartMovementMessage()
	{
		messageType = MessageType.CharacterMovementStarted;
	}
}

public class CharacterFinishedMovingMessage : Message
{
	public Character movingCharacter;
	public List<Character> waitingObjects = new List<Character>();
	public System.Action waitingAction;

	public CharacterFinishedMovingMessage(System.Action a)
	{
		messageType = MessageType.CharacterFinishedMoving;
		waitingAction = a;
	}

	public void RemoveWaitingObject(Character character)
	{
		waitingObjects.Remove(character);
		if (waitingObjects.Count == 0)
		{
			waitingAction();
		}
	}
}

public class CharacterCritCheckMessage : Message
{
	public Character attacker;
	public Character defender;
	public int critThreshold = 20;

	public CharacterCritCheckMessage()
	{
		messageType = MessageType.CharacterCritCheck;
	}
}

public class CharacterCritMessage : Message
{
	public Character attacker;
	public Character defender;

	public CharacterCritMessage()
	{
		messageType = MessageType.CharacterCrit;
	}
}

public class CardStartMessage : Message
{
	public Character character;
	public Card card;

	public CardStartMessage()
	{
		messageType = MessageType.CardStart;
	}
}

public class CombatStartMessage : Message
{
	public CombatStartMessage()
	{
		messageType = MessageType.CombatStart;
	}
}

public class CharacterTakingBurnDamageMessage : Message
{
	public Character character;
	public bool autoFail = false;

	public CharacterTakingBurnDamageMessage()
	{
		messageType = MessageType.CharacterTakingBurnDamage;
	}
}

public class MessagePump : SceneSingleton<MessagePump>
{
    List<IMessageReceiver> listeners = new List<IMessageReceiver>();

    public void AddListener(IMessageReceiver receiver)
	{
		listeners.Add(receiver);
	}

	public void RemoveListener(IMessageReceiver receiver)
	{
		listeners.Remove(receiver);
	}

	public void SendMessage(Message message)
	{
		foreach (IMessageReceiver receiver in listeners)
		{
			receiver.ReceiveMessage(message);
		}
	}
}
