using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformHeight : MonoBehaviour
{
    public RectTransform rectTransform;
    public float offset;

    RectTransform ourRect;

    void Start()
	{
        ourRect = (RectTransform)transform;
	}

    // Update is called once per frame
    void Update()
    {
        ourRect.sizeDelta = new Vector2(ourRect.sizeDelta.x, rectTransform.sizeDelta.y + offset);
    }
}
