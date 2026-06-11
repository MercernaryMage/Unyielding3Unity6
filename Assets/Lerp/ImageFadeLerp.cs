using UnityEngine;
using UnityEngine.UI;

public class ImageFadeLerp : Lerper
{
    public float a0;
    public float a1;

    public Image[] images;

    public void Init()
    {
        images = GetComponentsInChildren<Image>();
        foreach (Image r in images)
        {

            r.color = new Color(r.color.r, r.color.g, r.color.b, a0);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float aT = a0 + (a1 - a0) * t;



        foreach (Image r in images)
        {
            if (r)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, aT);
            }
        }

        PostUpdate();
    }
}
