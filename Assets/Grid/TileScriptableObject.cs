using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileScriptableObject : ScriptableObject
{
    public bool enterable;
    public bool blocksLOS;
    public bool empty;
}
