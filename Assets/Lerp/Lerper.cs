using System;
using UnityEngine;

public enum InterpolationType
{
    Simple,
    EaseIn,
    EaseOut,
    Smooth,
    Smoother
}

public class Lerper : MonoBehaviour
{
    public float currentTime = 0.0f;
    public float runTime;
    public InterpolationType type;
    public float t;
    public Action callbackFunction;
    public string funcName;
    public bool destroy;
    bool firstUpdate = true;
    public bool ignoreBadFPS = false;

    protected void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            return;
        }
        if (ignoreBadFPS)
        {
            if (Time.deltaTime > .05f)
            {
                currentTime += .02f;
            }
            else
            {
                currentTime += Time.deltaTime;
            }
            
        }
        else
        {
            currentTime += Time.deltaTime;
        }
        
        t = currentTime / runTime;
        if (type == InterpolationType.EaseIn)
        {
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
        }
        else if (type == InterpolationType.EaseOut)
        {
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        }
        else if (type == InterpolationType.Smooth)
        {
            t = t * t * (3f - 2f * t);
        }
        else if (type == InterpolationType.Smoother)
        {
            t = t * t * t * (t * (6f * t - 15f) + 10f);
        }

        t = Mathf.Min(t, 1.0f);
    }

    protected void PostUpdate()
    {
        if (currentTime >= runTime)
        {
            End();
            if (callbackFunction != null)
            {
                callbackFunction();
            }
            Destroy(this);
        }
    }

    protected virtual void End()
    {
        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
