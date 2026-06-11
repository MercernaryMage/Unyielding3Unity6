using UnityEngine;

public class CanvasGroupLerper : Lerper
{

    public float a0;
    public float a1;

    CanvasGroup canvasGroup;

    public void Init()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = a0;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float a = a0 + (a1 - a0) * t;
        canvasGroup.alpha = a;

        PostUpdate();
    }

    public void Out(float run)
    {
        a0 = 1;
        a1 = 0;
        runTime = run;
        Init();
    }

    public void In(float run)
    {
        a0 = 0;
        a1 = 1;
        runTime = run;
        Init();
    }
}
