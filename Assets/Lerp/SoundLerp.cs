using UnityEngine;



public class SoundLerp : Lerper
{

    public float v0;
    public float v1;

    public AudioSource source;

    public void Init()
    {

        source = GetComponent<AudioSource>();
        source.volume = v0;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        float vT = v0 + (v1 - v0) * t;

        source.volume = vT;

        PostUpdate();
    }
}
