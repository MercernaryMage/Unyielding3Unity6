using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLossControl : SceneSingleton<WinLossControl>
{
	public GameObject winScreen;
	public GameObject defeatScreen;

	public void PlayerWins()
	{
		Invoke("PlayerWinsActual", 1.5f);
	}

	public void PlayerWinsActual()
	{
		winScreen.SetActive(true);
	}

	public void EnemyWins()
	{
		Invoke("EnemyWinsActual", 1.5f);
	}

	public void EnemyWinsActual()
	{
		defeatScreen.SetActive(true);
	}

	public void WinClicked()
	{
		++FlowControl.currentLevel;
		MapSetScriptableObject mapSet = Resources.Load<MapSetScriptableObject>($"MapSets/{FlowControl.mapSetName}");

		FlowControl.Instance.RunPostFightCode(mapSet);

		if (FlowControl.currentLevel == mapSet.configurations.Count)
		{
			FlowControl.Instance.RunSetCompleteCode(mapSet);
			SceneManager.LoadScene("Town");
		}
		else
		{
			SceneManager.LoadScene("Combat");
		}
	}

	public void LoseClicked()
	{

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PlayerWinsActual();
		}
	}
}
