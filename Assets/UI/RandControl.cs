using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandControl : MonoBehaviour
{
    public bool overrideRand;
    public int seed;

    void Awake()
    {
        if (!Application.isEditor)
		{
            return;
		}
        if (overrideRand)
		{
            Random.InitState(seed);
			
		}
        else
		{
            int currentSeed = Random.Range(0, 1000000000);
            Debug.Log("Rand seed is: " + currentSeed);
            Random.InitState(currentSeed);
		}
    }

}
