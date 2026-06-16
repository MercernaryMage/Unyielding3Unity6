using UnityEngine;

public class DebugUnlockMap : MonoBehaviour
{
	private void Awake()
	{
		if (!DebugStartup.init)
		{
			OverlandManager.Instance.overrideUnlockedAll = true;
		}
	}
}
