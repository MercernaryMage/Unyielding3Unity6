using System.Collections.Generic;
using UnityEngine;

public class StopAllEffectsAfterDelay : MonoBehaviour
{
    public float runTime = 3f;
    public List<ParticleSystem> systems;

    // Update is called once per frame
    void Update()
    {
        runTime -= Time.deltaTime;
		if (runTime < 0)
		{
			foreach (ParticleSystem system in systems)
			{
				system.Stop();
			}
			Destroy(this);
		}
	}
}
