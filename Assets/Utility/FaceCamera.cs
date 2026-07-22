using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void OnEnable()
    {
        Face();
    }

    // Update is called once per frame
    void Update()
    {
        Face();
    }

    void Face()
    {
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
