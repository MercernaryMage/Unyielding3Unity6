using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueHelper : MonoBehaviour
{
    public Tile tile;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Setup", 0);
    }

    void Setup()
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position + Vector3.up * .3f, Vector3.down);
        for (int i = 0; i < raycastHits.Length; i++)
        {
            Tile t = raycastHits[i].transform.gameObject.GetComponent<Tile>();
            if (t != null)
            {
                tile = t;

                return;
            }
        }
        Debug.LogError("We didn't find a tile!");
    }



    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + Vector3.up * .3f, Vector3.down);
    }
}
