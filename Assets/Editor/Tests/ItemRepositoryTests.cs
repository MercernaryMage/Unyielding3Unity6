using NUnit.Framework;
using UnityEngine;

public class ItemRepositoryTests
{
    [Test]
    public void ItemCollection_HasNoNullItems()
    {
        GameObject prefab = Resources.Load<GameObject>("ItemCollection");
        Assert.IsNotNull(prefab, "ItemCollection prefab not found in Resources");

        ItemCollection itemCollection = prefab.GetComponent<ItemCollection>();
        Assert.IsNotNull(itemCollection, "ItemCollection component missing from prefab");

        Assert.IsNotNull(itemCollection.items, "Items list is null");

        for (int i = 0; i < itemCollection.items.Count; i++)
        {
            Assert.IsNotNull(itemCollection.items[i], $"Item at index {i} is null");
        }
    }
}
