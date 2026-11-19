using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatDucker : PropDucker
{
	float finalValue = .3f;

	override public void DuckActual()
	{
		foreach (GameObject obj in objectsToDuck)
		{
			MatLerp matLerp = obj.AddComponent<MatLerp>();
			matLerp.startValue = 1;
			matLerp.endValue = finalValue;
			matLerp.runTime = .3f;
			matLerp.Init(obj.GetComponent<MeshRenderer>());
		}
	}

	override public void UnduckActual()
	{
		foreach (GameObject obj in objectsToDuck)
		{
			MatLerp matLerp = obj.AddComponent<MatLerp>();
			matLerp.startValue = finalValue;
			matLerp.endValue = 1;
			matLerp.runTime = .3f;
			matLerp.Init(obj.GetComponent<MeshRenderer>());
		}
	}
}
