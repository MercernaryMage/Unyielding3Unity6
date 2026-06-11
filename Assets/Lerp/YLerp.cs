using UnityEngine;

public class YLerp : Lerper
{

    public float y0;
    public float y1;

    float startY;

    public void Init()
    {
        startY = transform.localPosition.y;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float y = y0 + (y1 - y0) * t;
        transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);

        PostUpdate();
    }

    public void ToZero()
    {
        y0 = 1;
        y1 = 0;
        runTime = .3f;
        Init();
    }

    public void ToOne()
    {
        y0 = 0;
        y1 = 1;
        runTime = .3f;
        Init();
    }
}
