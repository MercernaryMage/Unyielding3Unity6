using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAttackWiggle : MonoBehaviour
{
	public EffectScriptableObject effect;
	public Action completeCallback;

	Vector3 startPosition;

	float startTime = .5f;
	float time;
	bool secondaryRunning = true;
	float secondaryTime;

	float magnitude = 1;

	private void Start()
	{
		time = startTime;
		secondaryTime = startTime / 4;
		startPosition = transform.position;
	}



	private void Update()
	{
		time -= Time.deltaTime;
		secondaryTime -= Time.deltaTime;
		float inverseTime = startTime - time;
		float x = magnitude * (time / startTime) * Mathf.Sin((inverseTime /startTime) * Mathf.PI * 2);
		transform.position = startPosition + transform.forward * x;
		if (secondaryTime <= 0 && secondaryRunning)
		{
			secondaryRunning = false;
			GameObject obj = Instantiate(effect.prefab);
			obj.transform.position = transform.position + effect.offset;
			obj.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + effect.rotation);
			if (completeCallback != null)
			{
				completeCallback();
			}
		}
		if (time <= 0)
		{
			Destroy(this);
		}
	}
}
