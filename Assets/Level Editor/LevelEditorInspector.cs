using TMPro;
using UnityEngine;

public class LevelEditorInspector : MonoBehaviour
{
	public LevelEditorConfigurationManager configurationManager;

	public TextMeshProUGUI characterName;

	EmptyTile currentTile;

	public void Set(EmptyTile t)
	{
		currentTile = null;
		characterName.text = "";

		int placementIndex = configurationManager.GetPlacementIndex(t);
		if (placementIndex != -1)
		{
			currentTile = t;
			characterName.text = GetPlacementName(placementIndex);
		}
		else if (t.mosnterName != "")
		{
			currentTile = t;
			characterName.text = t.mosnterName;
		}
	}

	public void RotateClicked()
	{
		if (currentTile == null)
		{
			return;
		}

		if (configurationManager.RotatePlacementTile(currentTile))
		{
			return;
		}

		configurationManager.RotateCharacter(currentTile);
	}

	public void DeleteClicked()
	{
		if (currentTile == null)
		{
			return;
		}

		if (!configurationManager.DeleteCharacter(currentTile))
		{
			return;
		}

		currentTile = null;
		characterName.text = "";
	}

	public void DeletePlacementClicked()
	{
		if (currentTile == null)
		{
			return;
		}

		if (!configurationManager.RemovePlacementTile(currentTile))
		{
			return;
		}

		Set(currentTile);
	}

	string GetPlacementName(int i)
	{
		if (i < 4)
		{
			return GetHeroName(i);
		}
		return "Placement";
	}

	string GetHeroName(int i)
	{
		if (i == 0)
		{
			return "Lenette";
		}
		else if (i == 1)
		{
			return "David";
		}
		else if (i == 2)
		{
			return "Dane";
		}
		else
		{
			return "Cottn";
		}
	}
}
