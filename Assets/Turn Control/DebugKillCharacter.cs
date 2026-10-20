
using System.Collections.Generic;
using UnityEngine;

public class DebugKillCharacter : MonoBehaviour
{

	public void KillCharacter()
	{
		ActionController.Instance.KillCharacter(CharacterInspector.Instance.lastCharacter);
	}
}
