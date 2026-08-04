using System.Collections.Generic;
using UnityEngine;

public class SwatchRepository : Singleton<SwatchRepository>
{
	SwatchCollection swatchesReal;
	Dictionary<string, SwatchScriptableObject> swatches;

	private void Awake()
	{
		swatchesReal = Resources.Load<GameObject>("SwatchCollection").GetComponent<SwatchCollection>();
		swatches = new Dictionary<string, SwatchScriptableObject>();
		foreach (SwatchScriptableObject swatch in swatchesReal.swatches)
		{
			swatches[swatch.name] = swatch;
		}
	}

	public SwatchScriptableObject GetExactSwatch(string swatchName)
	{
		return swatches[swatchName];
	}

	public IReadOnlyList<SwatchScriptableObject> GetSwatches()
	{
		return swatchesReal.swatches.AsReadOnly();
	}
}
