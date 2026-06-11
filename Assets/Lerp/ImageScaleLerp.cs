using UnityEngine;
using UnityEngine.UI;

public class ImageScaleLerp : Lerper
{

    public Vector2 s0;
    public Vector2 s1;
    public Image image;

    public void Init()
    {
        image.rectTransform.sizeDelta = s0;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        Vector2 sT = s0 + (s1 - s0) * t;
        image.rectTransform.sizeDelta = sT;

        PostUpdate();
    }
}
