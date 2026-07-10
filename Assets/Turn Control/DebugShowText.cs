using UnityEngine;

public class DebugShowText : MonoBehaviour
{
	public GameObject textObject;
	private void Start()
	{
		MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour comp in components)
		{
			if (comp is DebugShowText)
			{
				continue;
			}
			if (comp is DebugKeyboardCommands)
			{
				continue;
			}

			if (comp.enabled)
			{
				textObject.SetActive(true);
				return;
			}			
		}
	}
}
