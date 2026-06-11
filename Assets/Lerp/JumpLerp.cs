using UnityEngine;

public class JumpLerp : Lerper
{
    public float height;
    float orginalHeight;

    float heightOffset;

    public void Init()
    {
        orginalHeight = transform.localPosition.y;
        heightOffset = (.25f * height);
    }

    new void Update()
    {
        base.Update();

        float val = (t - .5f) * (t - .5f);

        transform.localPosition = new Vector3(transform.localPosition.x, orginalHeight - height * val + heightOffset, 0);

        PostUpdate();
    }
}
