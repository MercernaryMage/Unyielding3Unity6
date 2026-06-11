using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlandItem : MonoBehaviour
{
    public MapSetScriptableObject mapSet;

    void Start()
	{
        bool value = (bool)FlowControl.Instance.GetType()
            .GetMethod(mapSet.lockedFunction)
            .Invoke(FlowControl.Instance, null);
		if (value)
		{
            enabled = false;
		}
	}
}
