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

		if (t.playerIndex != -1)
		{
			currentTile = t;
			characterName.text = GetHeroName(t.playerIndex);
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
