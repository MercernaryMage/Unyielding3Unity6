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
				if (i == (int)Tile.OverlayType.TileWarning)
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
			if (i == (int)Tile.OverlayType.TileWarning)
			{
				continue;
			}
			overlays[i].SetActive(false);
		}
	}

	public void TurnOffWarning()
	{
		overlays[(int)Tile.OverlayType.TileWarning].SetActive(false);
	}
}
