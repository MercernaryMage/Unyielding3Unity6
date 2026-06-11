using UnityEngine;
using UnityEngine.UI;

public class CanvasRendererFadeLerp : Lerper
{
    public float a0;
    public float a1;

    public CanvasRenderer[] renderers;

    public void Init()
    {
        renderers = GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer r in renderers)
        {

            r.SetAlpha(a0);
        }
    }

    // Update is called once per frame
    new void Update()
    {
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
}
