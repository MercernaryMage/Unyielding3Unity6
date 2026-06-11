using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    GameObject mainCamera;

    void Start()
    {
        mainCamera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform.position);
    }
}
