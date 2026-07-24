using System.Collections.Generic;
using UnityEngine;

public class ExplanationItemRepository : Singleton<ExplanationItemRepository>
{
	ExplanationItemCollection collectionReal;
	Dictionary<string, ExplanationIemScrptableObject> explanations;

	private void Awake()
	{
		collectionReal = Resources.Load<GameObject>("ExplanationItemCollection").GetComponent<ExplanationItemCollection>();
		explanations = new Dictionary<string, ExplanationIemScrptableObject>();
		foreach (ExplanationIemScrptableObject item in collectionReal.explanationItems)
		{
			explanations[item.explanationName] = item;
		}
	}

	public ExplanationIemScrptableObject GetExplanation(string explanationName)
	{
		if (explanations.ContainsKey(explanationName))
		{
			return explanations[explanationName];
		}
		return null;
	}
}
