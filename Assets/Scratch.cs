using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Scratch : MonoBehaviour
{
}

/*

 * 
 * When EndTurn is called, it returns a list of pending actions.
 * We then go through each pending action one at a time, doing them
 * After each one is done, we return to EndTurn.
 * 
*/
