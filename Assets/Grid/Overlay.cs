using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Overlay : MonoBehaviour
{
	public List<GameObject> overlays;

	public void Set(Tile.OverlayType type)
	{
        for (int i = 0; i < overlays.Count; ++i)
		{
			if (i == (int)type)
			{
				overlays[i].SetActive(true);
			}
			else
			{
				if (IsWarning(i))
				{
					continue;
				}
				overlays[i].SetActive(false);
			}
		}
	}

	public void TurnOffOverlay()
	{
		for (int i = 0; i < overlays.Count; ++i)
		{
			if (IsWarning(i))
			{
				continue;
			}
			overlays[i].SetActive(false);
		}
	}

	bool IsWarning(int index)
	{
		return index == (int)Tile.OverlayType.TileWarning || index == (int)Tile.OverlayType.TileWarningFollow;
	}

	public void TurnOffWarning(Tile.OverlayType type)
	{
		overlays[(int)type].SetActive(false);
	}
}
