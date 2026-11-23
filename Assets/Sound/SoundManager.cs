using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
	Dictionary<string, AudioSource> effectSources = new Dictionary<string, AudioSource>();

	public void PlaySound(string soundName)
	{
		if (effectSources.ContainsKey(soundName))
		{
			return;
		}
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = SoundRepository.Instance.GetClip(soundName);
		source.Play();
		effectSources.Add(soundName, source);
	}

	public void Update()
	{
		List<string> keys = effectSources.Keys.ToList();
		for (int i = 0; i < keys.Count; ++i)
		{
			if (effectSources.ContainsKey(keys[i]) && !effectSources[keys[i]].isPlaying)
			{
				Destroy(effectSources[keys[i]]);
				effectSources.Remove(keys[i]);
				--i;
			}
		}
	}
}
