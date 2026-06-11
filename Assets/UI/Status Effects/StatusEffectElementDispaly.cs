using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusEffectElementDispaly : MonoBehaviour
{
	public TextMeshProUGUI effectName;
	public TextMeshProUGUI effectText;

	public void Set(StatusEffect effect)
	{
		effectName.text = effect.GetDisplayName();
		effectText.text = effect.GetEffectText();
	}

}
