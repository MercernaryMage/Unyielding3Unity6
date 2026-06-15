using TMPro;
using UnityEngine;

public class OverlandInspector : SceneSingleton<OverlandInspector>
{
    public TextMeshProUGUI areaNameText;
    public TextMeshProUGUI lockTypeText;
    public TextMeshProUGUI lockStatusText;

	private void Start()
	{
		Set(null, false);
	}


	public void Set(MapSetScriptableObject set, bool locked)
	{
		if (set == null)
		{
			areaNameText.text = "";
			lockTypeText.text = "";
			lockStatusText.text = "";
			return;
		}
		areaNameText.text = set.setName;
		lockTypeText.text = set.rewardText;
		if (locked)
		{
			lockStatusText.text = "locked";
		}
		else
		{
			lockStatusText.text = "unlocked";
		}
	}
}
