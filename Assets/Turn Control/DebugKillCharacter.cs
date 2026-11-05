
using System.Collections.Generic;
using UnityEngine;

public class DebugKillCharacter : MonoBehaviour
{
#if UNITY_EDITOR
	public void KillCharacter()
	{
		ActionController.Instance.KillCharacter(CharacterInspector.Instance.lastCharacter);
	}
#endif
}
