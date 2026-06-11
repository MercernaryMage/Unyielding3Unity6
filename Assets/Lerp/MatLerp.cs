using UnityEngine;

public class MatLerp : Lerper
{
    public float startValue;
    public float endValue;
    public Renderer render;
    MaterialPropertyBlock block;

    public void Init(Renderer r)
    {
        render = r;
        block = new MaterialPropertyBlock();
        render.GetPropertyBlock(block);
    }

    new void Update()
    {
        base.Update();
        float a = startValue + (endValue - startValue) * t;
        block.SetFloat("_Alpha", a);
        render.SetPropertyBlock(block);

        PostUpdate();
    }
}
