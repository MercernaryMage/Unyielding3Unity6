using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillCheckController : SceneSingleton<SkillCheckController>
{
	public enum SkillCheckType
	{
		Prowess,
		Cunning

	}

	public GameObject content;

	public List<CanvasRenderer> canvasRenderers = new List<CanvasRenderer>();

    public TextMeshProUGUI title;
    public TextMeshProUGUI difficulty;
    public TextMeshProUGUI check;
    public TextMeshProUGUI bonus;
    public TextMeshProUGUI result;

	SkillCheckType storedType;
	int storedDifficulty;
	Action<bool> storedAction;
	Character storedCharacter;
	int storedResult;
	int storedRoll;
	int storedBonus;
	int step = -1;
	float time = 0;
	bool running = false;
	bool resultValue;

	public void Set(SkillCheckType type, int difficulty, Character c, Action<bool> action)
	{
		content.gameObject.SetActive(true);
		running = true;
		step = -1;
		storedType = type;
		storedDifficulty = difficulty;
		storedCharacter = c;
		storedAction = action;

		if (type == SkillCheckType.Cunning)
		{
			title.text = "Cunning Skill Check";
			bonus.text = $"{c.displayName}'s bonus: {c.characterDefinition.cunning}";
		}
		else
		{
			title.text = "Prowess Skill Check";
			bonus.text = $"{c.displayName}'s bonus: {c.characterDefinition.prowess}";
		}
		this.difficulty.text = $"Difficulty: {difficulty}";
		storedRoll = Util.RollDice(1, 20);
		check.text = $"Skill Check Result: {storedRoll}";
		storedBonus = type == SkillCheckType.Cunning ? c.characterDefinition.cunning : c.characterDefinition.prowess;
		storedResult = storedRoll + storedBonus;
		if (storedResult >= difficulty)
		{
			resultValue = true;
			result.text = "SUCCESS";
		}
		else
		{
			resultValue = false;
			result.text = "FAILURE";
		}

		foreach (CanvasRenderer renderer in canvasRenderers)
		{
			renderer.SetAlpha(0);
		}
	}

	private void Update()
	{
		if (!running)
		{
			return;
		}
		time += Time.deltaTime;
		if (time > 1)
		{
			time = 0;
			++step;
			if (step >= canvasRenderers.Count)
			{
				content.SetActive(false);
				running = false;
				string checkName = storedType == SkillCheckType.Cunning ? "cunning" : "prowess";
				string outcome = resultValue ? "succeeds" : "fails";
				string rollLine = $"Roll: {storedRoll} (1d20) + {storedBonus} ({checkName}) vs {storedDifficulty}";
				CombatLogControl.Instance.AddEntry($"{storedCharacter.displayName} {outcome} a {checkName} check ({storedResult} v {storedDifficulty})", new List<string> { rollLine });
				storedAction.Invoke(resultValue);
				return;
			}
			canvasRenderers[step].SetAlpha(1);	
		}
	}
}
