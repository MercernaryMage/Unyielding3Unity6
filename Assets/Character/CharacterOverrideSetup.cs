using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Override", order = 1)]
public class CharacterOverrideSetup : ScriptableObject
{
    public List<ItemScriptableObject> weaponSlots;
	public List<ItemScriptableObject> adventages;
}
