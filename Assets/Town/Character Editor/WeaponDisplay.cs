using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
	public Image weaponIcon;
	public TextMeshProUGUI weaponName;
	public TextMeshProUGUI weaponRange;
	public TextMeshProUGUI weaponDamage;
	public TextMeshProUGUI weaponTags;

	Item item;
	ItemScriptableObject itemScriptableObject;

	public void Set(ItemScriptableObject scriptable, Item i)
	{
		item = i;
		itemScriptableObject = scriptable;
		//weaponIcon.
		weaponName.text = itemScriptableObject.displayName;
		if (itemScriptableObject.actions[0].range > 0)
		{
			weaponRange.text = itemScriptableObject.actions[0].range.ToString();
		}
		else
		{
			weaponRange.text = "";
		}
		List<(int, int, int, ActionPattern.DamageType)> damages = Util.ParseDiceString(itemScriptableObject.actions[0].damageString);
		if (damages[0].Item1 > 0)
		{
			weaponDamage.text = $"{damages[0].Item1}D{damages[0].Item2} + {damages[0].Item3}";
		}
		else
		{
			weaponDamage.text = "";
		}
		weaponTags.text = "";
	}

	public void Click()
	{
		AdvantageSelectionPane.Instance.WeaponDisplayClicked(itemScriptableObject, item);
	}
}
