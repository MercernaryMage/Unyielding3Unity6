using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class TurnEvent
{
	public Action action;
	public int tick;
	private Action a;
	private int t;

	public TurnEvent(Action a, int t)
	{
		this.a = a;
		this.t = t;
	}
}

public class TurnEventController : SceneSingleton<TurnEventController>
{
	public List<TurnEvent> events;
	int currentTick;

	public void AddEvent(Action a, int t)
	{
		events.Add(new TurnEvent(a, t));
	}

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
				events[i].action();
				events.RemoveAt(i);
				return;
			}
		}

	}
}
