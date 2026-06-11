using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispLifetime : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    enum lifestage
    {
        birth,
        life,
        death
    }

    float birthTime = 1;
    float lifeTime = 2 * Mathf.PI;
    float deathTime = 1;

    float currentTime = 0;

    float startDeathValue;

    lifestage currentStage = lifestage.birth;

    MaterialPropertyBlock block;

    float period = 1;

    private void Start()
    {
        meshRenderer.material = new Material(meshRenderer.material);
        block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);

        period = Util.Range(.5f, 5.5f);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentStage == lifestage.birth)
        {
            float t = currentTime / birthTime;
            block.SetFloat("_Alpha", t);
            
            if (currentTime >= birthTime)
            {
                currentStage = lifestage.life;
                currentTime = 0;
            }
        }
        else if (currentStage == lifestage.life)
        {
            float amplitude = .4f * Mathf.Cos(currentTime * period) + .6f;
            block.SetFloat("_Alpha", amplitude);
            if (currentTime >= lifeTime)
            {
                currentStage = lifestage.death;
                currentTime = 0;
                startDeathValue = amplitude;
            }
        }
        else
        {
            float t = currentTime / deathTime;
            block.SetFloat("_Alpha", (1-t) * startDeathValue);

            if (currentTime >= deathTime)
            {
                Destroy(transform.gameObject);
            }
        }
        meshRenderer.SetPropertyBlock(block);
    }
}
