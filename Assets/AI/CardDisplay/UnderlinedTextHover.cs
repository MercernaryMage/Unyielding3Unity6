using UnityEngine;
using TMPro;

public class UnderlinedTextHover : MonoBehaviour
{
	static UnderlinedTextHover activeHover;

	TextMeshProUGUI text;
	Canvas canvas;

	public float hoverPadding = 6f;

	int hoveredRunStart = -1;

	void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
		canvas = GetComponentInParent<Canvas>();
	}

	void Update()
	{
		if (text == null)
		{
			return;
		}

		Camera camera = null;
		if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
		{
			camera = canvas.worldCamera;
		}

		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(text.rectTransform, Input.mousePosition, camera, out localPoint);
		int charIndex = FindHoveredUnderlinedCharacter(localPoint);

		if (charIndex == -1)
		{
			if (hoveredRunStart != -1)
			{
				HideTooltip();
				hoveredRunStart = -1;
			}
			return;
		}

		int runStart = GetUnderlinedRunStart(charIndex);
		if (runStart == hoveredRunStart)
		{
			return;
		}
		hoveredRunStart = runStart;

		string word = GetUnderlinedRun(charIndex);
		ExplanationIemScrptableObject explanation = ExplanationItemRepository.Instance.GetExplanation(word);
		if (explanation != null)
		{
			ShowTooltip(explanation.explanationName, explanation.explanationContent);
		}
		else
		{
			HideTooltip();
		}
	}

	void OnDisable()
	{
		if (hoveredRunStart != -1)
		{
			HideTooltip();
			hoveredRunStart = -1;
		}
	}

	void ShowTooltip(string titleText, string bodyText)
	{
		activeHover = this;
		GenericTooltip.Instance.Set(titleText, bodyText);
	}

	void HideTooltip()
	{
		if (activeHover != this)
		{
			return;
		}
		activeHover = null;
		if (GenericTooltip.Instance != null)
		{
			GenericTooltip.Instance.Hide();
		}
	}

	int FindHoveredUnderlinedCharacter(Vector2 localPoint)
	{
		TMP_TextInfo textInfo = text.textInfo;
		int best = -1;
		float bestDistance = float.MaxValue;
		for (int i = 0; i < textInfo.characterCount; ++i)
		{
			TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
			if (!charInfo.isVisible)
			{
				continue;
			}
			if ((charInfo.style & FontStyles.Underline) != FontStyles.Underline)
			{
				continue;
			}

			TMP_LineInfo lineInfo = textInfo.lineInfo[charInfo.lineNumber];
			float xMin = charInfo.bottomLeft.x - hoverPadding;
			float xMax = charInfo.topRight.x + hoverPadding;
			float yMin = lineInfo.descender - hoverPadding;
			float yMax = lineInfo.ascender + hoverPadding;

			if (localPoint.x < xMin || localPoint.x > xMax || localPoint.y < yMin || localPoint.y > yMax)
			{
				continue;
			}

			float centerX = (charInfo.bottomLeft.x + charInfo.topRight.x) * 0.5f;
			float centerY = (lineInfo.descender + lineInfo.ascender) * 0.5f;
			float dx = localPoint.x - centerX;
			float dy = localPoint.y - centerY;
			float distance = dx * dx + dy * dy;
			if (distance < bestDistance)
			{
				bestDistance = distance;
				best = i;
			}
		}
		return best;
	}

	int GetUnderlinedRunStart(int charIndex)
	{
		int start = charIndex;
		while (start > 0 && IsUnderlined(start - 1))
		{
			--start;
		}
		return start;
	}

	bool IsUnderlined(int charIndex)
	{
		if (charIndex < 0 || charIndex >= text.textInfo.characterCount)
		{
			return false;
		}
		FontStyles style = text.textInfo.characterInfo[charIndex].style;
		return (style & FontStyles.Underline) == FontStyles.Underline;
	}

	string GetUnderlinedRun(int charIndex)
	{
		TMP_TextInfo textInfo = text.textInfo;
		int start = GetUnderlinedRunStart(charIndex);
		int end = charIndex;
		while (end < textInfo.characterCount - 1 && IsUnderlined(end + 1))
		{
			++end;
		}

		string result = "";
		for (int i = start; i <= end; ++i)
		{
			result += textInfo.characterInfo[i].character;
		}
		return result;
	}
}
