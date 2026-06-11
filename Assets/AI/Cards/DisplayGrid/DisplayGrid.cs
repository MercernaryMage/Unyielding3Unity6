using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayGrid : SceneSingleton<DisplayGrid>
{
	int height = 8;
	int width = 12;

	public GameObject content;
	public GameObject target;

	List<GameObject> displayTiles = new List<GameObject>();
	List<DisplayGridInternalObject> data;

	public List<GameObject> displayPrefabs;

	List<GameObject> createdObjects = new List<GameObject>();

	public enum DisplayGridObject
	{
		Size1Enemy,
		Size2Enemy,
		Size2EnemyMove,
		Size3Enemy,
		Size4Enemy,
		EffectedTile,
		KnockbackArrow,
	}

	enum DisplayGridObjectInternal
	{
		None,
		Size1Enemy0,
		Size2Enemy0,
		Size2Enemy1,
		Size2Enemy2,
		Size2Enemy3,
		EffectedTile,
		KnockbackArrow,
	}

	public enum DisplayGridDirection
	{
		None,
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest
	}

	class DisplayGridInternalObject
	{
		public DisplayGridInternalObject()
		{
			displayElement = DisplayGridObjectInternal.None;
			direction = DisplayGridDirection.None;
		}
		public DisplayGridInternalObject(DisplayGridObjectInternal o, DisplayGridDirection d)
		{
			displayElement = o;
			direction = d;
		}
		public DisplayGridObjectInternal displayElement;
		public DisplayGridDirection direction;
		public Color color = Color.white;
	}

	public void Add(DisplayGridObject displayGridObject, List<System.Tuple<int, int>> coords)
	{
		foreach (System.Tuple<int, int> tuple in coords)
		{
			Add(displayGridObject, tuple.Item1, tuple.Item2);
		}
	}

	public void Add(DisplayGridObject displayGridObject, int x, int y)
	{
		Add(displayGridObject, DisplayGridDirection.None, x, y);
	}

	public void Add(DisplayGridObject displayGridObject, DisplayGridDirection direction, int x, int y)
	{
		if (displayGridObject == DisplayGridObject.Size1Enemy)
		{
			data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size1Enemy0, direction);
		}
		if (displayGridObject == DisplayGridObject.Size2Enemy ||
			displayGridObject == DisplayGridObject.Size2EnemyMove)
		{
			if (direction == DisplayGridDirection.North)
			{
				data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy2, direction);
				data[x + 1 + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy3, direction);
				data[x + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy0, direction);
				data[x + 1 + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy1, direction);
			}
			else if (direction == DisplayGridDirection.South)
			{
				data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy1, direction);
				data[x + 1 + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy0, direction);
				data[x + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy3, direction);
				data[x + 1 + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy2, direction);
			}
			else if (direction == DisplayGridDirection.West)
			{
				data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy0, direction);
				data[x + 1 + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy2, direction);
				data[x + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy1, direction);
				data[x + 1 + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy3, direction);
			}
			else
			{
				data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy3, direction);
				data[x + 1 + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy1, direction);
				data[x + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy2, direction);
				data[x + 1 + (y + 1) * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.Size2Enemy0, direction);
			}
			if (displayGridObject == DisplayGridObject.Size2EnemyMove)
			{
				data[x + y * width].color = Color.blue;
				data[x + 1 + y * width].color = Color.blue;
				data[x + (y + 1) * width].color = Color.blue;
				data[x + 1 + (y + 1) * width].color = Color.blue;
			}
		}
		if (displayGridObject == DisplayGridObject.EffectedTile)
		{
			data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.EffectedTile, direction);
		}
		if (displayGridObject == DisplayGridObject.KnockbackArrow)
		{
			data[x + y * width] = new DisplayGridInternalObject(DisplayGridObjectInternal.KnockbackArrow, direction);
		}
	}
	
    public void Clear(int newWidth, int newHeight)
	{
		RectTransform rect = (RectTransform)content.transform;
		rect.sizeDelta = new Vector2(12.5f * newWidth, 16 * newHeight);
		target.transform.localPosition = new Vector3(0, -8 * newHeight);
		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
		}
		createdObjects.Clear();
		width = newWidth;
		height = newHeight;
		data = new List<DisplayGridInternalObject>();
		for(int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				data.Add(new DisplayGridInternalObject());
			}
		}

	}

	public void Hide()
	{
		content.SetActive(false);
	}

    public void Show()
	{
		content.SetActive(true);
		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				GameObject obj = Instantiate(displayPrefabs[(int)data[x + y * width].displayElement]);
				obj.transform.SetParent(target.transform);
				obj.transform.localPosition = new Vector3(x * 16, y * 16, 0);
				if (data[x + y * width].direction == DisplayGridDirection.South)
				{
					obj.transform.rotation = Quaternion.Euler(0, 0, 180);
				}
				else if (data[x + y * width].direction == DisplayGridDirection.East)
				{
					obj.transform.rotation = Quaternion.Euler(0, 0, 270);
				}
				else if (data[x + y * width].direction == DisplayGridDirection.West)
				{
					obj.transform.rotation = Quaternion.Euler(0, 0, 90);
				}
				obj.GetComponent<Image>().color = data[x + y * width].color;
				createdObjects.Add(obj);
			}
		}
	}

	public void SetIndex(int index)
	{
		content.transform.SetSiblingIndex(index);
	}	
}
