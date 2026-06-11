using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEditorCharacter : MonoBehaviour
{
    public Image characterPortrait;
    public TextMeshProUGUI characterName;
	StorageCharacter storageCharacter;

	public void Set(StorageCharacter s)
	{
		storageCharacter = s;
		characterPortrait.sprite = s.characterDefintion.battlePortrait;
		characterName.text = s.characterDefintion.displayName;
	}

	public void Click()
	{
		CharacterEditor.Instance.SetToCharacter(storageCharacter);
	}
}
