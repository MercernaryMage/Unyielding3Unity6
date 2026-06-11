using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverlandManager : MonoBehaviour
{
	public void BackToTown()
	{
		SceneManager.LoadScene("Town");
	}
}
