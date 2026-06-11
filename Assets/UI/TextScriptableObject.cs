using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Text", order = 1)]
public class TextScriptableObject : ScriptableObject
{
    [TextArea]
    public string text;
}
