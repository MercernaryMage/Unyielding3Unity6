using UnityEngine;
using UnityEngine.UI;


public class RectLerper : Lerper
{

    public Vector2 v0;
    public Vector2 v1;

    public VerticalLayoutGroup verticalLayoutGroup;


    public void Init()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = v0;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        Vector2 vT = v0 + (v1 - v0) * Mathf.Min(t, 1.0f);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = vT;

        if (verticalLayoutGroup)
        {
            verticalLayoutGroup.SetLayoutVertical();
        }

        PostUpdate();
    }
}
