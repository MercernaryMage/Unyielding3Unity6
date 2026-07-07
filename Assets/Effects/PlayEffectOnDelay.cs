using UnityEngine;

public class PlayEffectOnDelay : MonoBehaviour
{
	float delay;
	EffectScriptableObject effect;
	Vector3 scale;
	Vector3 position;
	Quaternion rotation;
	bool effectPlayed = false;
	System.Action action;
	float actionDelay;

	public void Create(EffectScriptableObject e, float d, Vector3 p, Vector3 s, Quaternion r)
	{
		delay = d;
		effect = e;
		scale = s;
		rotation = r;
		position = p;
	}
	public void Create(EffectScriptableObject e, float d, Vector3 p, Vector3 s, Quaternion r, System.Action a, float aD)
	{
		Create(e,d,p,s,r);
		action = a;
		actionDelay = aD;
	}

	void Update()
	{
		delay -= Time.deltaTime;
		if (delay <= 0)
		{
			if (effectPlayed)
			{
				action();
				Destroy(this);
			}
            else
            {
				ShowEffect();
				if (action == null)
				{
					Destroy(this);
				}
				else
				{
					delay = actionDelay;
					effectPlayed = true;
				}
			}

		}
	}

	void ShowEffect()
	{
		GameObject obj = Instantiate(effect.prefab);
		obj.transform.SetParent(transform);
		obj.transform.position = position + effect.offset;
		obj.transform.localScale = scale;
		obj.transform.rotation = rotation;
	}
}
