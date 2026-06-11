using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TraitDisplay : MonoBehaviour
{
    public TextMeshProUGUI traitTitle;
    public TextMeshProUGUI traitDescription;

	public void Set(TraitScriptableObject traitScriptableObject)
	{
		traitTitle.text = traitScriptableObject.displayName;
		traitDescription.text = traitScriptableObject.description;
	}
}
