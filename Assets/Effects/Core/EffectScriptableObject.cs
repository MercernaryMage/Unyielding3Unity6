using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect", order = 1)]

public class EffectScriptableObject : ScriptableObject
{
	public enum EffectPosition
	{
		origin,
		target,
		halfway,
		bone
	}

	public enum EffectTiming
	{
		immediate,
		wiggleApex
	}

	public bool wiggle = true;
	public EffectTiming timing;
	public float delay;
	public float runTime;
	public Vector3 offset;
	public Vector3 rotation;
	public GameObject prefab;
	public EffectPosition position;
	public string bone;
}
