using TMPro;
using UnityEngine;

public class TMPOutlineCopy : MonoBehaviour
{
    TextMeshProUGUI ourField;
    TextMeshProUGUI parentField;

    void Awake()
    {
        ourField = GetComponent<TextMeshProUGUI>();
        parentField = transform.parent.GetComponent<TextMeshProUGUI>();

        Transform parent = transform.parent;
        int index = parent.GetSiblingIndex();
        transform.SetParent(parent.parent, false);
        transform.SetSiblingIndex(index);

        ourField.enabled = true;
        ourField.font = parentField.font;
        ourField.transform.localPosition = parentField.transform.localPosition;
        ((RectTransform)ourField.transform).sizeDelta = ((RectTransform)parentField.transform).sizeDelta;
	}

    void Update()
    {
        ourField.text = parentField.text;
        ourField.fontSize = parentField.fontSize;
    }
}
