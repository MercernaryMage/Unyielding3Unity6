using UnityEngine;

public class SpriteFadeLerp : Lerper
{

    public float a0;
    public float a1;

    public SpriteRenderer[] renderers;
    public bool dontFadeChildren = false;

    public void Init()
    {
        if (dontFadeChildren)
        {
            renderers = GetComponents<SpriteRenderer>();
        }
        else
        {
            renderers = GetComponentsInChildren<SpriteRenderer>();
        }
        foreach (SpriteRenderer r in renderers)
        {
            r.color = new Color(r.color.r, r.color.g, r.color.b, a0);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float aT = a0 + (a1 - a0) * t;



        foreach (SpriteRenderer r in renderers)
        {
            if (r)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, aT);
            }
        }

        PostUpdate();
    }
}
