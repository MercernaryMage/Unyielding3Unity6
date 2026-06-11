using UnityEngine;

public class DecoScalar : DecoHider
{
    Vector3 savedScale;
    float targetScale;
    float startScale;
    float elapsed;
    bool animating;
    const float Duration = 0.3f;

    void Start()
    {
        savedScale = transform.localScale;
        targetScale = savedScale.x;
        startScale = savedScale.x;
    }

    void Update()
    {
        if (!animating)
        {
            return;
        }

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / Duration);
        float s = Mathf.Lerp(startScale, targetScale, t);
        transform.localScale = new Vector3(s, s, s);

        if (t >= 1f)
        {
            animating = false;
        }
    }

    public override void Hide()
    {
        startScale = transform.localScale.x;
        targetScale = 0f;
        elapsed = 0f;
        animating = true;
    }

    public override void Show()
    {
        startScale = transform.localScale.x;
        targetScale = savedScale.x;
        elapsed = 0f;
        animating = true;
    }
}
