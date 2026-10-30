using TMPro;
using UnityEngine;

public class TownPlayerEquipmentWarning : MonoBehaviour
{
    public TextMeshProUGUI warningField;
	public GameObject content;

	public void Set(string str)
	{
		content.SetActive(true);
		warningField.text = str;
	}

	public void BackClicked()
	{
		content.SetActive(false);
	}

	public void ForwardClicked()
	{
		TownManager.Instance.ForwardClicked();
	}
}
