using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Reaction", order = 1)]
public class ReactionScriptableObject : ScriptableObject
{
    public CardScriptableObject normalReaction;
    public CardScriptableObject aggravatedReaction;
}
