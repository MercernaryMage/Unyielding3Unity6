using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraFocusCharacter : MonoBehaviour
{
	public Character character;

	void Update()
    {
		SelectionManager.Instance.CenterCameraOnCharacter(character);
	}
}
