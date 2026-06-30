using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMinder : MonoBehaviour
{
    public List<ParticleSystem> systems;
    public List<Animator> animators;
    bool ready;
    bool hasAnimators;

    private void Start()
    {
        hasAnimators = animators.Count > 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool live = false;
        foreach (ParticleSystem system in systems)
        {
            if (system.particleCount > 0)
            {
                ready = true;
                live = true;
                
            }
        }
        bool aliveAnimators = false;
        foreach (Animator animator in animators)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                aliveAnimators = true;
            }
        }
        if (ready && !live
            || hasAnimators && !aliveAnimators)
        {
            Destroy(gameObject);
        }
    }
}
