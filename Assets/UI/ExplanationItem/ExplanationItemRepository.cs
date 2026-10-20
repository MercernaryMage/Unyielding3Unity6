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
			explanations[item.name] = item;
		}
	}

	public ExplanationIemScrptableObject GetExplanation(string explanationKey)
	{
		if (explanations.ContainsKey(explanationKey))
		{
			return explanations[explanationKey];
		}
		Debug.LogError($"Missing Explanation: {explanationKey}");
		return null;
	}
}
