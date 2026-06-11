using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubclassDisplay : MonoBehaviour
{
    public TextMeshProUGUI subclassName;
    public TextMeshProUGUI lieDisplay;

	public void Set(StorageCharacter storageCharacter, int index)
	{
		subclassName.text = storageCharacter.characterDefintion.subclasses[index].subclassName;
		string fancyString = "";
		fancyString += $"• {storageCharacter.characterDefintion.subclasses[index].lie1}\n";
		fancyString += $"• {storageCharacter.characterDefintion.subclasses[index].lie2}\n";
		fancyString += $"• {storageCharacter.characterDefintion.subclasses[index].lie3}";
		lieDisplay.text = fancyString;
	}
}
