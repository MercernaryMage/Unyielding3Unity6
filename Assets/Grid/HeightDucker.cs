using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightDucker : PropDucker
{
	override public void DuckActual()
	{
		foreach (GameObject obj in objectsToDuck)
		{
			IndepedentScaleLerp scaleLerp = obj.AddComponent<IndepedentScaleLerp>();
			scaleLerp.runTime = .3f;
			scaleLerp.startScale = new Vector3(1, 1, 1);
			scaleLerp.endScale = new Vector3(1, .2f, 1);
			scaleLerp.Init();
		}
	}

	override public void UnduckActual()
	{
		foreach (GameObject obj in objectsToDuck)
		{
			IndepedentScaleLerp scaleLerp = obj.AddComponent<IndepedentScaleLerp>();
			scaleLerp.runTime = .3f;
			scaleLerp.endScale = new Vector3(1, 1, 1);
			scaleLerp.startScale = new Vector3(1, .2f, 1);
			scaleLerp.Init();
		}
	}
}
