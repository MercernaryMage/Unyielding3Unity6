using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMinder : MonoBehaviour
{
    ParticleSystem[] systems;
    void Start()
    {

        systems = GetComponentsInChildren<ParticleSystem>();
		//Invoke(nameof(Stop), 0);
    }

// 	public void Stop()
// 	{
// 		foreach (ParticleSystem pSystem in systems)
// 		{
// 			pSystem.Stop();
// 		}
// 	}

	// Update is called once per frame
	void Update()
	{
		foreach (ParticleSystem pSystem in systems)
		{
            if (pSystem.particleCount > 0)
			{
                return;
			}
		}
        Destroy(gameObject);
	}
}
