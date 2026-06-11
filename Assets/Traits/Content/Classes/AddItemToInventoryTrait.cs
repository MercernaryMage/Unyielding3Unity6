using System.Collections.Generic;
using UnityEngine;

public class AddItemToInventoryTrait : Trait
{
	public override void Start()
	{
		base.Start();
		character.temporaryItems.Add(Item.CreateItem(ItemRepository.Instance.GetExactItem(scriptableObject.stringParams[0])));
	}
}
