using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWiggle : MonoBehaviour
{
	public GameObject secondaryAnimationObject;
	public Action midwayCallback;
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
		if (completeCallback != null)
		{
			completeCallback();
		}
	}

	/*
	 * Hey.  This code is not working as designed, but as intended.
	 * We didn't end up using the compelteCallback concept or the midway.
	 * They are left as a maybe somde day.
	 * 
	 */

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
			GameObject obj = Instantiate(secondaryAnimationObject);
			obj.transform.position = transform.position + new Vector3(0, .5f, 0);
			obj.transform.rotation = transform.rotation;
		}
		if (time <= 0)
		{
			Destroy(this);
		}
	}
}
