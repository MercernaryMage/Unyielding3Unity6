using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[System.Serializable]
public class ActionPattern
{
    public enum TargetType
	{
        Hero,
        Monster,
        Geo,
		Self,
        All,
        Directions
	}

	public enum AoEType
	{
        None,
		Cone,
        Burst,
        Line,
        Directions
	}

	public enum DamageType
	{
        None,
        Physical,
        Magic,
        Burning
	}

	[System.Serializable]
	public class InstantAction
	{
		public string actionName;
		public string disqualiferFunction;
	}

	[System.Serializable]
    public class TargetedAction
	{
        public TargetType targetType;
        public string actionName;
        public string disqualiferFunction;
    }

    [System.Serializable]
	public class ActionCost
	{
        public int movement;
        public int actions;
        public int energy;
	}

	public static string GetActionCostString(ActionCost actionCost)
	{
        string outString = "";
        if (actionCost.actions > 0)
        {
            outString = $"{actionCost.actions}A";
        }
        else if (actionCost.movement > 0)
        {
            return $"{actionCost.movement}\nM";
		}
		if (actionCost.energy > 0)
		{
            outString += $"{actionCost.energy}E";
		}
		return outString;
	}

    public string displayName;
    public ActionCost cost;
    public bool attack;
    public bool physical;
    public bool ranged;
    public int range = 1;
    public int threatRange = 0;
    public int accuracy = 0;
    public string damageString;
    public AoEType aoeType;
    public int aoeRange;
    public int chargesTaken;
    public string disqualifierFunc;
    public int intParam;
    public string stringParam;
	public List<string> keywords;
    public List<string> postActionCommands;
	public string postDamageFunctionName;
	public bool useInstantAction;
	public InstantAction instantAction;
	public bool useTargetedAction;
    public TargetedAction targetedAction;
	[TextArea]
    public string actionDescription;
	public string actionDescriptionFunction;
    public string uniqueName;
    public ActionPattern storedPattern;
    public Character forcedTarget;
    public bool cannotBeUsedOnOthers;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemScriptableObject : ScriptableObject
{    
    public string displayName;
    public Sprite itemImage;
	public int weight;
	public bool weapon;
    public WeaponSlotType slotType;
    [TextArea]
    public string description;
	public int charges;
    public int armor;
    public List<ActionPattern> actions;
    public string keywords;
    public bool displayable = true;
}
