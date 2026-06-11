using UnityEngine;

public class FadeLerp : Lerper
{

    public float a0;
    public float a1;

    public CanvasRenderer[] renderers;

    public bool useLateUpdate = false;

    public void Init()
    {
        renderers = GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer r in renderers)
        {
            r.SetAlpha(a0);
        }
    }

    new void Update()
    {
        if (useLateUpdate)
        {
            return;
        }
        base.Update();

        float aT = a0 + (a1 - a0) * t;


        foreach (CanvasRenderer r in renderers)
        {
            if (r)
            {
                r.SetAlpha(aT);
            }
        }

        PostUpdate();
    }

    private void LateUpdate()
    {
        if (!useLateUpdate)
        {
            return;
        }
        base.Update();

        float aT = a0 + (a1 - a0) * t;


        renderers = GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer r in renderers)
        {
            if (r)
            {
                r.SetAlpha(aT);
            }
        }

        PostUpdate();
    }

    public void BasicFadeIn()
    {
        a0 = 0;
        a1 = 1;
        runTime = .3f;
        Init();
    }

    public void BasicFadeOut()
    {
        a0 = 1;
        a1 = 0;
        runTime = .3f;
        Init();
    }
}
