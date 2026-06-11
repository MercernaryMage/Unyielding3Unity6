using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalWisp : MonoBehaviour
{
    float speed = .6f;
    Vector3 startPosition;
    float time;
    float amp = 1;

    void Start()
    {
        startPosition = transform.position;
        time = Util.Range(0, 2 * Mathf.PI);
        amp = Util.Range(.2f, .8f);
    }

    private void Update()
    {
        time += Time.deltaTime;
        transform.position += speed * Time.deltaTime * Vector3.up;
        transform.position = new Vector3(startPosition.x + amp * Mathf.Cos(time),
                                        transform.position.y,
                                        startPosition.z + amp * Mathf.Sin(time));
    }
}
