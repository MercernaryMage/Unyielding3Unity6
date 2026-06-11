using UnityEngine;

public class PropertyValueLerper : Lerper
{

    public float p0;
    public float p1;

    public string propertyName;

    public Renderer[] rendererWithPropertys;

    public MaterialPropertyBlock block;

    public void Init()
    {
        block = new MaterialPropertyBlock();
        block.SetFloat(propertyName, p0);
        for (int i = 0; i < rendererWithPropertys.Length; ++i)
        {
            rendererWithPropertys[i].SetPropertyBlock(block);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float pT = p0 + (p1 - p0) * t;
        block.SetFloat(propertyName, pT);
        for (int i = 0; i < rendererWithPropertys.Length; ++i)
        {
            rendererWithPropertys[i].SetPropertyBlock(block);
        }

        PostUpdate();
    }
}
