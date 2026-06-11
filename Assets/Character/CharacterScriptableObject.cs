using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Subclass
{
    public string subclassName;
    public string lie1;
	public string lie2;
	public string lie3;
}

public enum WeaponSlotType
{
	Support,
	Medium,
	Heavy
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterScriptableObject : ScriptableObject
{
    public string displayName;
    public GameObject model;
    public int size = 1;
    public int movement = 6;
    public int armor;
    public int toughness;
    public int maxHP;
    public int maxEnergy;
    public int maxThreshold = 10;
    public int evasion;
    public bool isMinion;

    public int maxDetermination;

    public bool isScannable = true;

    public List<WeaponSlotType> slots;

    public SlotSetup[] weapons = new SlotSetup[3] 
    {
        new SlotSetup(), 
        new SlotSetup(),
        new SlotSetup()
    };
	public List<ItemScriptableObject> advantages; //starting advantages
    public int cunning;
    public int prowess;

    public List<CardScriptableObject> cards;
    public List<ReactionScriptableObject> reactions;

    public Sprite bigPortrait;
    public Sprite battlePortrait;

    public List<TraitScriptableObject> traits;
    public List<Subclass> subclasses;
}

[Serializable]
public class SlotSetup
{
	public ItemScriptableObject item;
}
