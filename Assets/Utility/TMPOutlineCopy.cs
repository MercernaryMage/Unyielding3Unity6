using TMPro;
using UnityEngine;

public class TMPOutlineCopy : MonoBehaviour
{
    public bool overrideColor = false;
    public Color overrideOutlineColor = Color.black;
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
        ourField.fontStyle = parentField.fontStyle;
        ((RectTransform)ourField.transform).sizeDelta = ((RectTransform)parentField.transform).sizeDelta;

        if (overrideColor)
        {
            ourField.outlineColor = overrideOutlineColor;
        }
	}

    void Update()
    {
        ourField.text = parentField.text;
        ourField.fontSize = parentField.fontSize;
    }
}
