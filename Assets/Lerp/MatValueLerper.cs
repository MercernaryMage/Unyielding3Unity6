using UnityEngine;

public class MatValueLerper : Lerper
{

    public float p0;
    public float p1;

    public string propertyName;
    public Material mat;


    public void Init()
    {
        
        mat.SetFloat(propertyName, p0);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float pT = p0 + (p1 - p0) * t;
        mat.SetFloat(propertyName, pT);

        PostUpdate();
    }
}
