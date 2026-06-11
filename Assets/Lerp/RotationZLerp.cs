using UnityEngine;

public class RotationZLerper : Lerper
{
    public float r0;
    public float r1;

    RectTransform rectTransform;

    public void Init()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.eulerAngles = new Vector3(0, 0, r0);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float rT = r0 + (r1 - r0) * t;
        rectTransform.eulerAngles = new Vector3(0, 0, rT);

        PostUpdate();
    }
}
