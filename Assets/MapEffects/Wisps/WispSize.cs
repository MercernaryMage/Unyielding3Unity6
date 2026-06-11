using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispSize : MonoBehaviour
{
    float time = .3f;

    float maxTime = .3f;

    void Update()
    {
        bool up = true;
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.Z))
        {
            up = false;
        }

        if (up)
        {
            time += Time.deltaTime;
        }
        else
        {
            time -= Time.deltaTime;
        }

        time = Mathf.Clamp(time, 0, maxTime);

        float p = time / maxTime;
        transform.localScale = Vector3.one * p;
    }


}
