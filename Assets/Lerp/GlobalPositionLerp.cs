using UnityEngine;

public class GlobalPositionLerp : Lerper
{

    public Vector3 p0;
    public Vector3 p1;
    public void Init()
    {
        transform.position = p0;
    }

    new void Update()
    {
        base.Update();

        Vector3 pT = p0 + (p1 - p0) * t;
        transform.position = pT;

        PostUpdate();
    }
}
