using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Trait", order = 1)]
public class TraitScriptableObject : ScriptableObject
{
	public string displayName;
	[TextArea]
	public string description;
	public string className;
	public List<int> intParams;
	public List<string> stringParams;
}
