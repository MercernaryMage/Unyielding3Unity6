using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwatchDisplay : MonoBehaviour, IPointerClickHandler
{
	SwatchScriptableObject swatch;
    public Image display;

	public void OnPointerClick(PointerEventData eventData)
	{
		LevelEditorManager.Instance.currentSwatch = swatch;
		LevelEditorManager.Instance.SwatchesClicked();
	}

	public void Set(SwatchScriptableObject s)
	{
		swatch = s;
		display.sprite = swatch.image;
	}
}
