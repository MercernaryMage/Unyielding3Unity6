using UnityEngine;

public class ScaleLerp : Lerper
{
    public float startScale;
    public float endScale;

    public void Init()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
    }

    new void Update()
    {
        base.Update();
        float sT = startScale + (endScale - startScale) * t;

        transform.localScale = new Vector3(sT, sT, sT);
        PostUpdate();
    }
}
