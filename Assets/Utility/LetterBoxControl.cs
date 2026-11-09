using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBoxControl : MonoBehaviour
{
    public RectTransform left;
    public RectTransform right;
    public RectTransform top;
    public RectTransform bottom;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)
        {
            return;
        }

        float targetaspect = 1920.0f / 1080.0f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;



        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            top.gameObject.SetActive(true);
            bottom.gameObject.SetActive(true);


        }
        else
        {
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
        }

    }

    
}
