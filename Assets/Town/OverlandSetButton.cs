using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OverlandSetButton : MonoBehaviour
{
	public MapSetScriptableObject mapSetScriptableObject;
	float time = 0;
	bool locked;

	public void Start()
	{
		locked = FlowControl.Instance.SetIsLocked(mapSetScriptableObject.lockedFunction) || FlowControl.Instance.SetIsComplete(mapSetScriptableObject.isComplete);
		if (OverlandManager.Instance.overrideUnlockedAll)
		{
			locked = false;
		}
		if (locked)
		{
			enabled = false;
		}
	}

	private void OnMouseEnter()
	{
		OverlandInspector.Instance.Set(mapSetScriptableObject, locked);
	}

	private void OnMouseExit()
	{
		OverlandInspector.Instance.Set(null, false);
	}

	private void OnMouseDown()
	{
		if (locked)
		{
			return;
		}
		//SET THE MAP TO BE LOADED
		FlowControl.currentLevel = 0;
		DebugSetLevel debugSetLevel = FindFirstObjectByType<DebugSetLevel>();
		if (debugSetLevel.enabled)
		{
			FlowControl.currentLevel = debugSetLevel.level;
		}
		FlowControl.mapSetName = mapSetScriptableObject.name;
		//SET THE CONFIGURATION TO BE LOADED
		SceneManager.LoadScene("Combat");
	}

	void Update()
	{
		time += Time.deltaTime;
		float scaleValue = 1 + .2f * Mathf.Cos(time * 2);
		transform.localScale = Vector3.one * scaleValue;
	}
}
