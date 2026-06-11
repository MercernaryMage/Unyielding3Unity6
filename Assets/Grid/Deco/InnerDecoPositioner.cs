using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDecoPositioner : MonoBehaviour
{
	float moveAmount = .5f;

	private void Start()
	{
		float xAmount = Random.Range(-moveAmount, moveAmount);
		float yAmount = Random.Range(-moveAmount, moveAmount);
		transform.localPosition += new Vector3(xAmount, 0, yAmount);
	}
}
