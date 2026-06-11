using UnityEngine;

public class RotationXLerper : Lerper
{
    public float r0;
    public float r1;

    Transform rectTransform;

    public void Init()
    {
        rectTransform = gameObject.transform;
        rectTransform.eulerAngles = new Vector3(r0, 0, 0);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float rT = r0 + (r1 - r0) * t;
        rectTransform.eulerAngles = new Vector3(rT, 0, 0);

        PostUpdate();
    }
}
