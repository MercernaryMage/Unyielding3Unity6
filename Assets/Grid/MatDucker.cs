using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatDucker : PropDucker
{
	override public void DuckActual()
	{
		foreach (GameObject obj in objectsToDuck)
		{
			MatLerp matLerp = obj.AddComponent<MatLerp>();
			matLerp.startValue = 1;
			matLerp.endValue = 0;
			matLerp.runTime = .3f;
			matLerp.Init(obj.GetComponent<MeshRenderer>());
		}
	}

	override public void UnduckActual()
	{
		foreach (GameObject obj in objectsToDuck)
		{
			MatLerp matLerp = obj.AddComponent<MatLerp>();
			matLerp.startValue = 0;
			matLerp.endValue = 1;
			matLerp.runTime = .3f;
			matLerp.Init(obj.GetComponent<MeshRenderer>());
		}
	}
}
