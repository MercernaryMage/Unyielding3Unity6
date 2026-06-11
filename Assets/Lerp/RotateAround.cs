using System;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
	public Vector3 rotateAroundPoint;
	public Vector3 rotateAroundAxis;

	public float angle;

	float runTime = .5f;
	float time;

	Vector3 startEuler;
	Vector3 startPosition;

	void Awake()
	{
		startEuler = transform.rotation.eulerAngles;
		startPosition = transform.position;

	}

	private void Update()
	{
		time += Time.unscaledDeltaTime;
		float t = time / runTime;
		t = Mathf.Clamp(t, 0, 1);
		transform.position = startPosition;
		transform.rotation = Quaternion.Euler(startEuler);
		transform.RotateAround(rotateAroundPoint, rotateAroundAxis, angle * t);
		if (time >= runTime)
		{
			BattleController.Instance.RunDuckers();
			Destroy(this);
		}
	}
}