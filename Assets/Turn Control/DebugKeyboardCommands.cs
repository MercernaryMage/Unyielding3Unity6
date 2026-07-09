using UnityEngine;

public class DebugKeyboardCommands : MonoBehaviour
{
    bool timeReduced = false;

    
    void Update()
    {
		if (Input.GetKeyUp(KeyCode.T) && Input.GetKey(KeyCode.LeftShift))
		{
			if (timeReduced)
			{
				Time.timeScale = 1f;
			}
			else
			{
				Time.timeScale = .1f;
			}
			timeReduced = !timeReduced;
		}
	}
}
