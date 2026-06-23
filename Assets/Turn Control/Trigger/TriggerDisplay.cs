using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerDisplay : SceneSingleton<TriggerDisplay>
{
    public GameObject displayPrefab;
    public Transform target;

	public GameObject content;

	Action storedContinue;
	Action storedAbandon;

	List<Tuple<GameObject, Trigger>> createdObjects = new List<Tuple<GameObject, Trigger>>();
	bool abandonded = false;
	bool showing = false;
	List<Tile> showingTiles = new List<Tile>();

	private void Start()
	{
	}

	public void ShowTriggerMenu(List<Trigger> triggers, Action c, Action a, 
		List<Tile> tiles)
	{
		showingTiles = tiles;
		foreach (Tile t in showingTiles)
		{
			t.HideAllOverlays();
			t.ShowOverlay(Tile.OverlayType.Selected);
		}
		ActionController.Instance.running = false;
		showing = true;
		abandonded = false;
		content.SetActive(true);
		foreach (Tuple<GameObject, Trigger> obj in createdObjects)
		{
			Destroy(obj.Item1);
		}
		createdObjects.Clear();

		storedContinue = c;
		storedAbandon = a;
		foreach (Trigger t in triggers)
		{
			CreateElement(t);
		}		
	}

	void CreateElement(Trigger trigger)
	{
		GameObject obj = Instantiate(displayPrefab);
		obj.GetComponent<TriggerDisplayElement>().Set(trigger);
		obj.transform.SetParent(target);
		obj.transform.SetAsFirstSibling();
		createdObjects.Add(new Tuple<GameObject, Trigger>(obj,trigger));
	}

	public void RemoveTrigger(Trigger trigger)
	{
		if (!showing)
		{
			return;
		}
		for (int i = 0; i < createdObjects.Count; ++i) // (Tuple<GameObject, Trigger> obj in createdObjects)
		{
			if (createdObjects[i].Item2.owningCharacter == trigger.owningCharacter)
			{
				Destroy(createdObjects[i].Item1);
				createdObjects.RemoveAt(i);
				--i;
			}
		}
		if (!abandonded)
		{
			if (createdObjects.Count == 0)
			{
				Continue();
			}
		}
	}

	public void Hide()
	{
		foreach (Tile t in showingTiles)
		{
			t.HideOverlay(Tile.OverlayType.Selected);
		}
		showing = false;
		content.SetActive(false);
		ActionController.Instance.running = true;
	}

	public void Continue()
	{
		Hide();
		storedContinue.Invoke();
	}

	public void Abandon()
	{
		if (!showing)
		{
			return;
		}
		Hide();
		storedAbandon.Invoke();
		abandonded = true;
	}
}
