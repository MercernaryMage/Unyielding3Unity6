using UnityEditor;
using UnityEngine;

public class UnyieldingMenu
{
	[MenuItem("Unyielding/Fill out all Content")]
	static void FillOutAllContent()
	{
		GameObject characterObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/CharacterRepositoryData.prefab");
		CharacterRepositoryData characterData = characterObj.GetComponent<CharacterRepositoryData>();
		CharacterRepositoryDataEditor.FilloutData(characterData);

		GameObject itemObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/ItemCollection.prefab");
		ItemCollection itemCollection = itemObj.GetComponent<ItemCollection>();
		ItemCollectionEditor.FilloutData(itemCollection);

		GameObject propObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/PropCollection.prefab");
		PropCollection propCollection = propObj.GetComponent<PropCollection>();
		PropCollectionEditor.FilloutData(propCollection);

		GameObject statusEffectIconObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/StatusEffectIconCollection.prefab");
		StatusEffectIconCollection statusEffectIconCollection = statusEffectIconObj.GetComponent<StatusEffectIconCollection>();
		StatusEffectIconCollectionEditor.FilloutData(statusEffectIconCollection);

		GameObject tileCollectionObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/TileCollection.prefab");
		TileCollection tileCollection = tileCollectionObj.GetComponent<TileCollection>();
		TileCollectionEditor.FilloutData(tileCollection);

		GameObject tileSOCollectionObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/TileScriptableObjectCollection.prefab");
		TileScriptableObjectCollection tileSOCollection = tileSOCollectionObj.GetComponent<TileScriptableObjectCollection>();
		TileScriptableObjectCollectionEditor.FilloutData(tileSOCollection);
	}
}
