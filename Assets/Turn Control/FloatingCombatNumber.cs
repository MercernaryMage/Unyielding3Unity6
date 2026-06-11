using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingCombatNumber : MonoBehaviour
{
    public TextMeshProUGUI number;
    float lifeTime = 3;
    bool marked = false;

    // Start is called before the first frame update
    public void Set(Vector3 position, string num)
	{
        number.text = num;
        transform.position = position;
	}

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * Vector3.up * 1;
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0 && !marked)
		{
            marked = true;
            FadeLerp fadeLerp = gameObject.AddComponent<FadeLerp>();
            fadeLerp.a0 = 1;
            fadeLerp.a1 = 0;
            fadeLerp.runTime = .3f;
            fadeLerp.destroy = true;
            fadeLerp.Init();
        }
    }
}
