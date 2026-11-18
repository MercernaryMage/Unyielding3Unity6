using UnityEngine;
using UnityEngine.Rendering.UI;

public class GameMenu : MonoBehaviour
{
	public GameObject content;

	public void ResumeClicked()
	{
		content.SetActive(false);
	}

	public void ExitGameClicked()
	{
		Application.Quit();
	}

	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			content.SetActive(!content.activeInHierarchy);	
		}
	}
}
