using UnityEngine;
using UnityEngine.UI;

public class ColorLerp : Lerper
{

    public Color c0;
    public Color c1;

    public Text text;
    public Image image;

    public void Init()
    {
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float r = c0.r + (c1.r - c0.r) * t;
        float g = c0.g + (c1.g - c0.g) * t;
        float b = c0.b + (c1.b - c0.b) * t;
        float a = c0.a + (c1.a - c0.a) * t;
        Color cT = new Color(r, g, b, a);
        if (text)
        {
            text.color = cT;
        }
        if (image)
        {
            image.color = cT;
        }

        PostUpdate();
    }
}
