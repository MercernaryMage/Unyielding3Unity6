using UnityEngine;
using System;
using System.Collections.Generic;

public class TurnEvent
{
	public Action action;
	public int tick;

	public TurnEvent(Action a, int t)
	{
		action = a;
		tick = t;
	}
}

public class TurnEventController : SceneSingleton<TurnEventController>
{
	public List<TurnEvent> events = new List<TurnEvent>();
	int currentTick;

	public void AddEvent(Action a, int t)
	{
		events.Add(new TurnEvent(a, t));
	}

	//Run every event scheduled for this tick before the tick's character is allowed to go.
	//Events are not inline: each event's action must call Pump() once it has finished so the
	//next one can start, and the last Pump() hands control back to TurnControl.
	public void PumpStart(int tick)
	{
		currentTick = tick;
		Pump();
	}

	public void Pump()
	{
		for (int i = 0; i < events.Count; ++i)
		{
			if (events[i].tick == currentTick)
			{
				Action action = events[i].action;
				events.RemoveAt(i);
				action();
				return;
			}
		}
		TurnControl.Instance.TurnEventControlFinished();
	}
}
