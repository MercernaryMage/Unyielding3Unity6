using TMPro;
using UnityEngine;

public class MonsterPickerButton : MonoBehaviour
{
    string monsterName;

    public TextMeshProUGUI buttonText;
	LevelEditorConfigurationManager configurationManager;

	public void Set(string s, LevelEditorConfigurationManager manager)
	{
		monsterName = s;
		configurationManager = manager;
		buttonText.text = monsterName;
	}

	public void Click()
	{
		configurationManager.MonsterButtonClicked(monsterName);
	}
}
