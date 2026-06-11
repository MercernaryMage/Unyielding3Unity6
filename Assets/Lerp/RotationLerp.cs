using UnityEngine;

public class RotationLerper : Lerper
{
    public float r0;
    public float r1;
    public float xOffset = 90.0f;

    RectTransform rectTransform;

    public void Init()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.eulerAngles = new Vector3(xOffset, 0, r0);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float rT = Mathf.LerpAngle(r0, r1, t);
        rectTransform.eulerAngles = new Vector3(xOffset, 0, rT);

        PostUpdate();
    }
}
