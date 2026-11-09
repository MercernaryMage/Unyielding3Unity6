using UnityEngine;
using UnityEngine.UI;

public class UltraWideSolution : MonoBehaviour
{
	void Awake()
	{
		if (Screen.width / Screen.height >= 2)
		{
			GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		}
	}
}
