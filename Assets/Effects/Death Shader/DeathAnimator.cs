using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimator : MonoBehaviour
{
    float time = 2;

    List<Material> materials = new List<Material>();
    MeshRenderer[] renderers;


    void Update()
    {
        float timeStep = 2 * Time.deltaTime;
        time -= timeStep;
        if (time < 1.5f)
        {
            transform.position -= timeStep * new Vector3(0, .1f, 0);
        }
        foreach (Material mat in materials)
        {
            mat.SetFloat("_height", time);
        }
        if (time <= -1)
        {
            Destroy(gameObject);
        }
    }

    public void Set(Material dyingMaterial, int size)
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in renderers)
        {
            meshRenderer.gameObject.layer = 0;
            Texture texture = meshRenderer.material.mainTexture;

            Material[] copyRef = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; ++i)
            {
                copyRef[i] = new Material(dyingMaterial);
                if (size == 2)
                {
                    time = 4;
                    copyRef[i].SetFloat("_height", 4);
                }
                copyRef[i].SetTexture("_main", texture);
                materials.Add(copyRef[i]);
            }
            meshRenderer.materials = copyRef;

            
            
        }
    }
}
