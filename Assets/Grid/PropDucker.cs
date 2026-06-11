using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PropDucker : MonoBehaviour
{
    public List<GameObject> objectsToDuck;

    public bool waitingForHit = false;
	public Collider ourCollider;
	bool ducked = false;

	void Start()
	{
		BattleController.Instance.AddDucker(this);
	}

	void Update()
	{
		if (waitingForHit)
		{
			waitingForHit = false;
			Unduck();
		}
	}

	abstract public void DuckActual();
	abstract public void UnduckActual();


	public void Duck()
	{
		waitingForHit = false;
		if (ducked)
		{
			return;
		}
		ducked = true;
		DuckActual();
	}

	public void Unduck()
	{
		if (!ducked)
		{
			return;
		}
		ducked = false;
		UnduckActual();
	}
}
