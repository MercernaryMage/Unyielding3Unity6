using UnityEngine;

public class PositionLerp : Lerper
{

    public Vector3 p0;
    public Vector3 p1;
    public void Init()
    {
        transform.localPosition = p0;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        Vector3 pT = p0 + (p1 - p0) * t;
        transform.localPosition = pT;

        PostUpdate();
    }
}
