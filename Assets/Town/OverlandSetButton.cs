using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OverlandSetButton : MonoBehaviour
{
	public MapSetScriptableObject mapSetScriptableObject;
	float time = 0;

	public void Start()
	{
		if (FlowControl.Instance.SetIsLocked(mapSetScriptableObject.lockedFunction) || FlowControl.Instance.SetIsComplete(mapSetScriptableObject.isComplete))
		{
			enabled = false;
		}
	}

	private void OnMouseDown()
	{
		if (mapSetScriptableObject.lockedFunction != "" && FlowControl.Instance.SetIsLocked(mapSetScriptableObject.lockedFunction))
		{
			return;
		}
		//SET THE MAP TO BE LOADED
		FlowControl.currentLevel = 0;
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
