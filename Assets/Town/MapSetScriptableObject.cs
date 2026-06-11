using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapSet", order = 1)]
public class MapSetScriptableObject : ScriptableObject
{
    public string setName;
    public string setDescription;
    public List<string> maps;
    public List<string> configurations;
    public string rewardText;
    public string lockedText;
	public string levelCompletedFunction;
	public string lockedFunction;
    public string onComplete;
    public string isComplete;
}
