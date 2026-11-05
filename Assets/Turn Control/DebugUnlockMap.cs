using UnityEngine;

public class DebugUnlockMap : MonoBehaviour
{
#if UNITY_EDITOR
	private void Awake()
	{
		if (!DebugStartup.init)
		{
			OverlandManager.Instance.overrideUnlockedAll = true;
		}
	}
#endif
}
