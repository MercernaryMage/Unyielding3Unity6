using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExplanationItem", order = 1)]
public class ExplanationIemScrptableObject : ScriptableObject
{
    public string explanationName;
	[TextArea]
	public string explanationContent;
}
