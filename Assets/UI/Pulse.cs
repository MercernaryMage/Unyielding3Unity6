using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float scale = 1f;
    Vector3 baseScale;
    float time = 0;
    void Start()
    {
        baseScale = transform.localScale;
    }

	private void OnEnable()
	{
        time = 0;
	}

	// Update is called once per frame
	void Update()
    {
        time += Time.deltaTime;
        float t = Mathf.Abs(Mathf.Sin(time));
        transform.localScale = baseScale + t * Vector3.one * scale;
    }
}
