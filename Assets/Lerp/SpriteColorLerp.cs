using UnityEngine;

public class SpriteColorLerp : Lerper
{

    public Color c0;
    public Color c1;

    public SpriteRenderer spriteRenderer;

    public void Init()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        c0 = spriteRenderer.color;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float r = c0.r + (c1.r - c0.r) * t;
        float g = c0.g + (c1.g - c0.g) * t;
        float b = c0.b + (c1.b - c0.b) * t;
        Color cT = new Color(r, g, b, spriteRenderer.color.a);
        spriteRenderer.color = cT;

        PostUpdate();
    }
}
