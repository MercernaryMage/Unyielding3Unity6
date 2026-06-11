using UnityEngine;

public class IndepedentScaleLerp : Lerper
{
    public Vector3 startScale;
    public Vector3 endScale;

    public void Init()
    {
        transform.localScale = startScale;
    }

    new void Update()
    {
        base.Update();
        transform.localScale = startScale + (endScale - startScale) * t;

        PostUpdate();
    }
}
