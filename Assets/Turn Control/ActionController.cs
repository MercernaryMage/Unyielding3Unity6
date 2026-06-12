using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static ActionController;
using static ActionPattern;
using static UnityEngine.GraphicsBuffer;

public class ActionController : SceneSingleton<ActionController>
{
	public GameObject swingAnimationObjectPrefab;
	public Character attackingCharacter;
	public Item currentItem;
	ActionPattern currentAction;
	List<Character> currentTargetsForAttack = new List<Character>();

	List<Tile> currentPossibleTiles = new List<Tile>();
	public bool running = false;
	bool internalRunning = false;
	bool wasStep = false;

	ActionPattern.AoEType aoeType;
	int aoeValue;

	List<Tile> markedTiles = new List<Tile>(); //THINGS THAT MIGHT BE TARGETED
	List<Tile> targetedTiles = new List<Tile>(); //THE ACTUAL AOE LIST

	public bool stepRunning = false;
	bool endActionIfNoTargets = false;
	Character stepCharacter;
	Item stepItem;
	ActionPattern stepAction;

	List<AttackResults> pendingReactionResults = new List<AttackResults>();

	Tile lastMousedOverTile = null;

	public List<Action> queuedActions = new List<Action>();
	public Material dyingMat;

	class ReactionCallback
	{
		public float time;
		public Action callback;
	}

	public class AttackResults
	{
		public bool hit;
		public bool fakeHit;
		public bool crit;
		public int damageDealt;
		public string outString;
	}

	List<ReactionCallback> reactionCallbacks = new List<ReactionCallback>();

	public void HideAttack()
	{
		foreach (Tile tile in currentPossibleTiles)
		{
			tile.HideOverlay(Tile.OverlayType.PossibleAttck);
		}
		foreach (Tile tile in targetedTiles)
		{
			tile.HideOverlay(Tile.OverlayType.Selected);
		}
		
		currentPossibleTiles.Clear();
		SetRunning(false);

		attackingCharacter = null;
	}

	void SetRunning(bool newRunning)
	{
		running = newRunning;
		SelectionManager.Instance.ShowCancelButton(newRunning);
	}

	public void ShowAttackableTiles(Character c, Item i, ActionPattern actionPattern)
	{
		MovementController.Instance.HideMovement();
		SetRunning(true);
		currentAction = actionPattern;
		currentItem = i;
		attackingCharacter = c;
		targetedTiles.Clear();
		internalRunning = false;

		aoeType = actionPattern.aoeType;

		if (aoeType == ActionPattern.AoEType.None)
		{
			List<Tile> startingTiles = TileGrid.Instance.FindCharacter(attackingCharacter);

			bool targetAllies = false;
			TargetType targetType = TargetType.Monster;
			if (currentAction.useTargetedAction)
			{
				targetType = currentAction.targetedAction.targetType;
				targetAllies = targetType == ActionPattern.TargetType.Hero;
			}

			List<Tile> tiles = GetStandardTiles(startingTiles, targetType, targetAllies);

			if (tiles.Count == 0)
			{
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(attackingCharacter, "No Target");
				CancelAttack(attackingCharacter);
			}
		}
	}

	public void MouseOverTile(Tile t)
	{
		lastMousedOverTile = t;
		foreach (Tile targetedTile in targetedTiles)
		{
			targetedTile.HideOverlay(Tile.OverlayType.Selected);
			if (markedTiles.Contains(targetedTile))
			{
				targetedTile.ShowOverlay(Tile.OverlayType.PossibleAttck);
			}
		}
		targetedTiles.Clear();
		bool isGeoTarget = currentAction != null && currentAction.useTargetedAction &&
			(currentAction.targetedAction.targetType == ActionPattern.TargetType.Geo ||
			currentAction.targetedAction.targetType == ActionPattern.TargetType.Directions);
		if (isGeoTarget)
		{
			if (markedTiles.Contains(t))
			{
				targetedTiles.Add(t);
			}
			return;
		}
		if (t.character == null)
		{
			return;
		}
		if (!markedTiles.Contains(t))
		{
			return;
		}
		List<Tile> fitTiles = TileGrid.Instance.FindCharacter(t.character);
		foreach (Tile fitTile in fitTiles)
		{
			targetedTiles.Add(fitTile);
			fitTile.HideOverlay(Tile.OverlayType.PossibleAttck);
			fitTile.ShowOverlay(Tile.OverlayType.Selected);
		}
	}

	public void HandleRightClick()
	{
		CancelAttack(attackingCharacter);
	}

	public bool HandleClick()
	{
		if (!internalRunning)
		{
			bool isGeoTarget = currentAction != null && currentAction.useTargetedAction &&
				(currentAction.targetedAction.targetType == ActionPattern.TargetType.Geo ||
				currentAction.targetedAction.targetType == ActionPattern.TargetType.Directions);

			if (aoeType != AoEType.None && !targetedTiles.Contains(lastMousedOverTile))
			{
				CancelAttack(attackingCharacter);
				return true;
			}


			if (!isGeoTarget)
			{
				targetedTiles.RemoveAll(o => o.character == null);
			}

			
			if (targetedTiles.Count != 0)
			{
				HandleActionStart();
			}
			else
			{
				if (!wasStep) //in step
				{
					CancelAttack(attackingCharacter);
				}
			}

			return true;
		}
		return false;
	}

	public void PlayAttackAnimation(GameObject wiggleObject, Action midwayCallback, Action completeCallback)
	{
		AttackWiggle attackWiggle = wiggleObject.AddComponent<AttackWiggle>();
		attackWiggle.secondaryAnimationObject = swingAnimationObjectPrefab;
		attackWiggle.completeCallback = completeCallback;
		attackWiggle.midwayCallback = midwayCallback;
	}

	public void PlayAttackAnimation(Character attacker, Action midwayCallback, Action completeCallback)
	{
		PlayAttackAnimation(attacker.token.gameObject, midwayCallback, completeCallback);
	}

	public void HandleEndOfAction()
	{
		if (reactionCallbacks.Count == 0)
		{
			EndAction();
		}
	}

	public void HandleActionStart()
	{
		lastMousedOverTile = null;
		internalRunning = true;
		wasStep = false;
		bool isGeoTarget = currentAction.useTargetedAction &&
			(currentAction.targetedAction.targetType == ActionPattern.TargetType.Geo ||
			currentAction.targetedAction.targetType == ActionPattern.TargetType.Directions);
		if (!isGeoTarget)
		{
			Tile tile = TileGrid.Instance.FindCharacter(attackingCharacter)[0];
			Tile closestDefenderTile = TileGrid.Instance.GetClosestCharacterTile(tile, targetedTiles[0].character);
			attackingCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(tile, closestDefenderTile));
		}
		TileGrid.Instance.HideAllTiles();
		////////////////////////// above here, do once
		if (currentAction.attack)
		{
			////////this stuff happens at the end, remember to queue up reactions to be done at this section
			PlayAttackAnimation(attackingCharacter, null, () =>
			{
				currentTargetsForAttack = new List<Character>();

				foreach (Tile targetTile in targetedTiles)
				{
					if (targetTile.character)
					{
						currentTargetsForAttack.Add(targetTile.character);
					}
				}

				currentTargetsForAttack = currentTargetsForAttack.Distinct().ToList();

				AttackTarget();
			});
		}
		else if (currentAction.useTargetedAction)
		{
			if (isGeoTarget)
			{
				ActionTypes.DoGeoTargetedAction(attackingCharacter, targetedTiles[0], currentAction);
			}
			else
			{
				ActionTypes.DoTargetedAction(attackingCharacter, targetedTiles[0].character, currentAction);
			}
		}
	}

	void AttackTarget()
	{
		Character target = currentTargetsForAttack[0];
		AttackProfile attackProfile = Util.GetAttackProfile(currentAction);
		if (!TileGrid.Instance.IsFacing(target, attackingCharacter))
		{
			attackProfile.isBackstab = true;
		}

		if (currentItem != null)
		{
			string[] tokens = currentItem.itemDefinition.keywords.Split(',');
			foreach (string token in tokens)
			{
				if (token.ContainsIgnoreCase("Breach"))
				{
					attackProfile.breach = true;
				}
				if (token.ContainsIgnoreCase("guaranteed"))
				{
					attackProfile.guaranteed = Util.ExtractValueFromString(token);
				}
			}
		}

		AttackCharacter(target, attackingCharacter, attackProfile);

		if (!string.IsNullOrEmpty(currentAction.postDamageFunctionName))
		{
			ActionTypes.DoPostAction(attackingCharacter, target, currentAction, currentItem, currentAction.postDamageFunctionName);
		}

		AttackCompleteMessage attackCompleteMessage = new AttackCompleteMessage();
		attackCompleteMessage.attacker = attackingCharacter;
		attackCompleteMessage.defender = target;

		MessagePump.Instance.SendMessage(attackCompleteMessage);

		if (attackCompleteMessage.raisedTriggers.Count > 0 && target.alive)
		{
			BattleController.playerHasControl = false;
			Action complete = () =>
			{
				currentTargetsForAttack.RemoveAt(0);
				PumpAttack();
			};
			Action abort = () =>
			{
				HandleEndOfAction();
			};
			TriggerDisplay.Instance.ShowTriggerMenu(attackCompleteMessage.raisedTriggers, complete, abort);
		}
		else
		{
			currentTargetsForAttack.RemoveAt(0);
			PumpAttack();
		}
	}

	void PumpAttack()
	{
		BattleController.playerHasControl = true;
		if (currentTargetsForAttack.Count == 0)
		{
			HandleEndOfAction();
		}
		else
		{
			AttackTarget();
		}
	}

	public void AttackCharacterActual(Character defender, Character attacker, AttackProfile profile)
	{

	}

	string GetAttackString(int attackRoll, int attackValue, AccuracyResult accuracyResult)
	{
		string attackString = $"Attack roll: {attackRoll} (1d20)";
		if (attackValue > 0)
		{
			attackString += $"+ {attackValue} (modifiers)";
		}
		if (accuracyResult.max != 0)
		{
			string accurarcyEntries = string.Join(", ", accuracyResult.rolls);
			if (accuracyResult.max > 0)
			{
				attackString += $" + {accuracyResult.max} (accuracy: {accurarcyEntries})";
			}
			else
			{
				attackString += $" - {-accuracyResult.max} (accuracy: {accurarcyEntries})";
			}
		}
		return attackString;
	}

	string GetAccuracyString(int total, string s)
	{
		if (s == "")
		{
			return "";
		}
		return $"Accuracy {total}: {s}";
	}

	string GenerateAccurarcyString(int profileValue)
	{
		if (profileValue == 0)
		{
			return "";
		}
		if (profileValue > 0)
		{
			return $"+{profileValue} (weapon)";
		}
		return $"{profileValue} (weapon)";
	}

	public AttackResults AttackCharacter(Character defender, Character attacker, AttackProfile profile)
	{
		string blurb;
		List<string> entries = new List<string>();
		AttackResults results = new AttackResults();
		int attackRoll = Util.RollDice(1, 20);

		CharacterAttackingMessage characterAttackingMessage = new CharacterAttackingMessage();
		characterAttackingMessage.defender = defender;
		characterAttackingMessage.attacker = attacker;
		characterAttackingMessage.pattern = currentAction;
		characterAttackingMessage.AddToAccuracyString(GenerateAccurarcyString(profile.accuracy));
		MessagePump.Instance.SendMessage(characterAttackingMessage);

		int totalAccurarcy = characterAttackingMessage.accuracy + profile.accuracy;
		entries.Add(GetAccuracyString(totalAccurarcy, characterAttackingMessage.accuracyString));
		AccuracyResult accuracyResult = GetMaximumAccuracy(totalAccurarcy);
		int attackValue = 0;

		entries.Add(GetAttackString(attackRoll, attackValue, accuracyResult));
		attackValue += attackRoll + accuracyResult.max;

		int effectiveEvasion = defender.currentEvasion;
		if (defender.gameObject.GetComponent<KnockedDown>() != null)
		{
			effectiveEvasion = Mathf.Min(effectiveEvasion, 5);
		}

		if (attackValue < effectiveEvasion || characterAttackingMessage.autoMiss)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(defender, "miss");

			CharacterMissMessage characterMissMessage = new CharacterMissMessage();
			characterMissMessage.defender = defender;
			characterMissMessage.attacker = attacker;
			MessagePump.Instance.SendMessage(characterMissMessage);
			if (profile.guaranteed > 0)
			{
				DamageCharacter(defender, attacker, profile, results);
				entries.Add(results.outString);
				blurb = $"{attacker.displayName}({attackValue}) misses {defender.displayName}({defender.characterDefinition.evasion}) for {results.damageDealt} damage";
			}
			else
			{
				blurb = $"{attacker.displayName}({attackValue}) misses {defender.displayName}({defender.characterDefinition.evasion})";
			}
		}
		else
		{
			results.hit = true;
			CharacterCritCheckMessage critCheckMessage = new CharacterCritCheckMessage();
			critCheckMessage.attacker = attacker;
			critCheckMessage.defender = defender;
			MessagePump.Instance.SendMessage(critCheckMessage);

			if (attackValue >= critCheckMessage.critThreshold ||
				characterAttackingMessage.autoCrit || profile.autoCrit)
			{
				entries.Add("Critical hit!");
				results.crit = true;
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(defender, "crit!");

				CharacterCritMessage critMessage = new CharacterCritMessage();
				critMessage.attacker = attacker;
				critMessage.defender = defender;
				MessagePump.Instance.SendMessage(critMessage);
			}
			DamageCharacter(defender, attacker, profile, results);
			entries.Add(results.outString);
			blurb = $"{attacker.displayName}({attackValue}) hits {defender.displayName}({defender.characterDefinition.evasion}) for {results.damageDealt} damage";
		}
		CombatLogControl.Instance.AddEntry(blurb, entries);
		return results;
	}

	int GetDamageResult(List<int> entries, int numberOfRolls)
	{
		int damage = 0;

		for (int i = 0; i < numberOfRolls; ++i)
		{
			damage += entries[i];
		}

		return damage;
	}

	public void DamageCharacter(Character defender, Character attacker, AttackProfile profile, AttackResults results)
	{
		int damage = 0;
		int repeat = 1;
		if (results.crit)
		{
			++repeat;
		}
		if (results.hit || results.fakeHit)
		{
			List<int> diceResults = new List<int>();
			for (int j = 0; j < repeat * profile.diceCount; ++j)
			{
				diceResults.Add(Util.RollDice(1, profile.diceFace));
			}
			diceResults = diceResults.OrderByDescending(x => x).ToList();
			damage = GetDamageResult(diceResults, profile.diceCount);
			if (repeat == 1)
			{
				results.outString += $"Damage roll: {damage} ({profile.diceCount}d{profile.diceFace})";
			}
			else
			{
				results.outString += $"Damage roll: {damage} ({profile.diceCount * repeat}d{profile.diceFace}) keep highest {profile.diceCount} ({string.Join(", ", diceResults)})";
			}
			if (profile.addedDamage != 0)
			{
				results.outString += $" + {profile.addedDamage} (weapon)";
			}
			damage += profile.addedDamage;
		}
		else
		{
			results.outString += $"{profile.guaranteed} guaranteed damage";
			damage = profile.guaranteed;
		}

		PreDamageDealtMessage preDamageDealtMessage = new PreDamageDealtMessage();
		preDamageDealtMessage.defender = defender;
		preDamageDealtMessage.attacker = attacker;
		preDamageDealtMessage.damage = damage;
		preDamageDealtMessage.ranged = profile.ranged;
		MessagePump.Instance.SendMessage(preDamageDealtMessage);
		damage = preDamageDealtMessage.damage;
		if (profile.isBackstab)
		{
			results.outString += $" + 1 (backstab)";
			damage += 1;
		}

		if (preDamageDealtMessage.hasResistance)
		{
			results.outString += $" / 2 (resistance)";
			damage /= 2;
		}
		if (defender.GetComponent<Vulnerable>() != null)
		{
			results.outString += $" * 2 (Vulnerable)";
			damage *= 2;
		}

		if (profile.preDamageAction != null)
		{
			profile.preDamageAction();
		}

		bool doReaction = false;
		if (!defender.hero && !profile.trigger)
		{
			if (defender.gameObject.GetComponent<KnockedDown>() != null)
			{
				defender.threshold += damage;
				if (defender.characterDefinition.maxThreshold != -1 && defender.threshold >= defender.characterDefinition.maxThreshold)
				{
					doReaction = true;
				}
			}
		}

		if (!profile.breach)
		{
			int originalArmor = defender.armor;
			defender.armor = Mathf.Max(defender.armor - damage, 0);
			if (originalArmor > 0)
			{
				results.outString += $" - {originalArmor} (armor)";
			}
			damage -= originalArmor;

			if (damage <= 0)
			{
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(defender, "0");
				if (doReaction)
				{
					HandleReaction(defender, results);
				}
				return;
			}

			if (defender.toughness != 0)
			{
				results.outString += $" - {defender.toughness} (toughness)";
			}


			damage -= defender.toughness;
			if (damage <= 0)
			{
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(defender, "0");
				if (doReaction)
				{
					HandleReaction(defender, results);
				}
				return;
			}
		}

		results.damageDealt = damage;
		defender.currentHP -= damage;
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(defender, damage.ToString());
		if (profile.damageType == DamageType.Burning)
		{
			StatusEffect.StatusEffectInitData statusEffectInitData = new StatusEffect.StatusEffectInitData();
			statusEffectInitData.magnitude = damage;
			Burning effect = (Burning)defender.AddStatusEffect(Type.GetType("Burning"), statusEffectInitData);
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(defender, $"{effect.magnitude} burning");
		}

		if (defender.currentHP <= 0)
		{
			int overflow = 0 - defender.currentHP;
			defender.currentHP = 0;
			if (profile.trigger)
			{
				TriggerDisplay.Instance.Abandon();
			}
			if (defender.storageCharacter != null && defender.storageCharacter.currentDetermination > 0)
			{
				defender.Downed(overflow);
				return;
			}
			else
			{
				KillCharacter(defender);
			}
		}
		else
		{
			DamageDealtMessage damageDealtMessage = new DamageDealtMessage();
			damageDealtMessage.defender = defender;
			damageDealtMessage.attacker = attacker;
			damageDealtMessage.damage = damage;
			MessagePump.Instance.SendMessage(damageDealtMessage);
			if (doReaction)
			{
				HandleReaction(defender, results);
			}
		}
	}

	public void KillCharacter(Character c)
	{
		TurnControl.Instance.UpdateSystem();
		c.Die();
		//Destroy(c.token.gameObject);
		DeathAnimator deathAnimator = c.token.AddComponent<DeathAnimator>();
		deathAnimator.Set(dyingMat, c.characterDefinition.size);
		TileGrid.Instance.RemoveCharacter(c);
	}

	void HandleReaction(Character defender, AttackResults results)
	{

		BattleController.playerHasControl = false;
		defender.threshold = 0;
		Character tempAttackingCharacter = attackingCharacter;

		ReactionCallback reactionCallback = new ReactionCallback();
		reactionCallback.time = 1.0f;
		reactionCallback.callback = () => { AIController.Instance.DoReaction(defender, tempAttackingCharacter); };
		reactionCallbacks.Add(reactionCallback);
	}

	public void DoSimpleAction(Character c, Item i, ActionPattern actionPattern)
	{
		SetRunning(true);
		currentAction = actionPattern;
		currentItem = i;
		attackingCharacter = c;

		EndAction();
	}

	public void OverrideCurrentAction(ActionPattern pattern)
	{
		currentAction = pattern;
	}

	public void DoQueuedAction()
	{
		Action a = queuedActions[0];
		queuedActions.RemoveAt(0);
		a();
	}

	public void EndAction()
	{
		if (queuedActions.Count > 0)
		{
			DoQueuedAction();
			return;
		}

		BattleController.playerHasControl = true;

		currentItem.used = true;

		ActionTypes.PayActionPatternCost(attackingCharacter, currentAction, currentItem);
		ActionTypes.PayActionPatternCharges(currentAction, currentItem);

		foreach (string str in currentAction.postActionCommands)
		{
			ActionTypes.DoPostAction(attackingCharacter, null, currentAction, currentItem, str);
		}

		UIController.Instance.UpdateAfterUsage();
		Character character = attackingCharacter;
		CancelAttack(character);
	}

	public void CancelAttack(Character character)
	{
		HideAttack();
		if (character.currentMovement > 0 && BattleController.playerHasControl)
		{
			MovementController.Instance.ShowMovement(character);
		}
		internalRunning = false;
	}

	public void CancelAttackFromEndTurn()
	{
		CancelAttack(attackingCharacter);
	}

	public void MoveCharacterFromImpact(Character c, List<Tile> impactTiles)
	{
		List<Tile> possibleTiles = new List<Tile>(TileGrid.Instance.tiles);
		List<Tile> startingTiles = TileGrid.Instance.FindCharacter(c);
		possibleTiles = possibleTiles.OrderBy(p => TileGrid.Distance(p, startingTiles[0])).ToList();
		foreach (Tile t in possibleTiles)
		{
			List<Tile> fitTiles = TileGrid.Instance.WhatTilesWouldCharacterTake(c, t);
			if (fitTiles == null)
			{
				continue;
			}
			bool failed = false;
			foreach (Tile fitTile in fitTiles)
			{
				if (impactTiles.Contains(fitTile))
				{
					failed = true;
					break;
				}
				if (fitTile.character != null)
				{
					failed = true;
				}
			}
			if (failed == false)
			{
				List<Tile> exitingTiles = TileGrid.Instance.FindCharacter(c);
				foreach (Tile exitTile in exitingTiles)
				{
					exitTile.character = null;
				}
				fitTiles = TileGrid.Instance.WhatTilesWouldCharacterTake(c, t);
				t.EnterTile(c);
				t.PlaceInTile(c);
				return;
			}
		}
	}

	public class AttackProfile
	{
		public AttackProfile(int c, int f, int ad)
		{
			diceCount = c;
			diceFace = f;
			addedDamage = ad;
		}
		public DamageType damageType;
		public int diceCount;
		public int diceFace;
		public int addedDamage;
		public int accuracy;
		public bool isBackstab;
		public bool breach;
		public bool trigger;
		public int guaranteed;
		public bool autoCrit;
		public bool ranged = false;
		public Action preDamageAction;
	}



	public bool KnockBack(Character c, List<Tile> points, int distance)
	{
		bool flip = distance < 0;
		distance = Mathf.Abs(distance);
		if (!c.alive)
		{
			return true;
		}
		List<Tile> fitTiles = TileGrid.Instance.FindCharacter(c);
		Vector2 characterPosition = TileGrid.FindCenter(fitTiles);
		Vector2 knockbackPointPosition = TileGrid.FindCenter(points);

		float angle = Vector2.SignedAngle(Vector2.right, characterPosition - knockbackPointPosition);
		if (angle < 0)
		{
			angle = 360 + angle;
		}
		if (flip)
		{
			angle += 180;
			angle = angle % 360;
		}
		List<Tuple<int, int>> directions = new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(1, 1),
			new Tuple<int, int>(0, 1),
			new Tuple<int, int>(-1, 1),
			new Tuple<int, int>(-1, 0),
			new Tuple<int, int>(-1, -1),
			new Tuple<int, int>(0, -1),
			new Tuple<int, int>(1, -1),
			new Tuple<int, int>(1, 0),
		};
		float min = 45 / 2.0f;
		float max = min + 45;
		bool found = false;
		int index = 0;
		for (; index < directions.Count - 1; ++index)
		{
			if (angle >= min && angle < max)
			{
				found = true;
				break;
			}
			else
			{
				min += 45;
				max += 45;
			}
		}
		if (!found)
		{
			index = directions.Count - 1;
		}
		Tuple<int, int> dir = directions[index];
		int finalIndex = 0;
		Tile startTile = fitTiles[0];
		for (int i = 1; i < distance + 1; ++i)
		{
			Tile newTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * i, startTile.y + dir.Item2 * i);
			if (newTile)
			{
				if (!TileGrid.Instance.WouldCharacterFitAtTile(c, newTile))
				{
					break;
				}
			}
			else
			{
				break;
			}
			++finalIndex;
		}
		Tile finalTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * finalIndex, startTile.y + dir.Item2 * finalIndex);
		TileGrid.Instance.MoveCharacterToTile(c, finalTile);


		return true;
	}

	public void BeginStep(Character c, Item i, ActionPattern actionPattern)
	{
		stepCharacter = c;
		stepItem = i;
		stepAction = actionPattern;
		stepRunning = true;
		wasStep = true;
		MovementController.Instance.onMoveComplete = () =>
		{
			stepRunning = false;
			endActionIfNoTargets = true;
			ShowAttackableTiles(stepCharacter, stepItem, stepAction);
		};
		MovementController.Instance.ShowMovement(c, 2);
		if (MovementController.Instance.currentPossibleTiles.Count == 0)
		{
			ExecuteStep();
		}
	}

	public void ExecuteStep()
	{
		MovementController.Instance.onMoveComplete = null;
		MovementController.Instance.HideMovement();
		stepRunning = false;
		ShowAttackableTiles(stepCharacter, stepItem, stepAction);
	}

	bool waitingForReaction = false;

	public void ReturnFromReaction()
	{
		waitingForReaction = false;
		reactionCallbacks.RemoveAt(0);
		if (reactionCallbacks.Count == 0)
		{
			if (attackingCharacter != null)
			{
				EndAction();
			}
			BattleController.playerHasControl = true;
		}
	}

	List<Tile> GetStandardTiles(List<Tile> startingTiles, TargetType targetType, bool targetAllies)
	{
		List<Tile> reachableTiles = new List<Tile>();
		foreach (Tile t in startingTiles)
		{
			foreach (Tile currentTile in TileGrid.Instance.tiles)
			{
				if (!TileGrid.Instance.DoesTileHaveLOSToTile(t, currentTile))
				{
					continue;
				}
				int distance = TileGrid.Distance(t, currentTile);
				if (distance <= currentAction.range)
				{
					if (targetType == ActionPattern.TargetType.Geo)
					{
						if (currentTile.guest == null)
						{
							reachableTiles.Add(currentTile);
						}
					}
					else if (currentTile.character && (targetType == ActionPattern.TargetType.All || currentTile.character.hero == targetAllies))
					{
						if (currentAction.useTargetedAction)
						{
							if (!ActionTypes.DoDisqualifierForAction(attackingCharacter, currentTile.character, currentAction).Item1)
							{
								continue;
							}
						}


						List<Tile> fitTiles = TileGrid.Instance.FindCharacter(currentTile.character);
						foreach (Tile fitTile in fitTiles)
						{
							reachableTiles.Add(fitTile);
						}

					}
				}
			}
		}
		return reachableTiles;
	}

	private void Update()
	{
		if (reactionCallbacks.Count > 0 && !waitingForReaction && currentTargetsForAttack.Count == 0)
		{
			reactionCallbacks[0].time -= Time.deltaTime;
			if (reactionCallbacks[0].time <= 0)
			{
				waitingForReaction = true;
				reactionCallbacks[0].callback();
			}
		}

		if (!running)
		{
			return;
		}
		if (aoeType != ActionPattern.AoEType.None)
		{
			if (!internalRunning)
			{
				ShowAoETargeting();
			}
		}
		else
		{
			foreach (Tile markedTile in markedTiles)
			{
				markedTile.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			markedTiles.Clear();
			List<Tile> reachableTiles = new List<Tile>();

			List<Tile> startingTiles = TileGrid.Instance.FindCharacter(attackingCharacter);

			bool targetAllies = false;
			TargetType targetType = TargetType.Monster;
			if (currentAction.useTargetedAction)
			{
				targetType = currentAction.targetedAction.targetType;
				targetAllies = targetType == ActionPattern.TargetType.Hero;
			}

			reachableTiles = GetStandardTiles(startingTiles, targetType, targetAllies);


			if (reachableTiles.Count == 0 && currentAction.useTargetedAction)
			{
				MovementController.Instance.ShowMovement(attackingCharacter);
				FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(attackingCharacter, "no targets");
				HideAttack();
			}

			markedTiles = reachableTiles.Distinct().ToList();
			if (endActionIfNoTargets)
			{
				endActionIfNoTargets = false;
				if (markedTiles.Count == 0)
				{
					EndAction();
					return;
				}
			}
			foreach (Tile reachableTile in markedTiles)
			{
				currentPossibleTiles.Add(reachableTile);
				reachableTile.ShowOverlay(Tile.OverlayType.PossibleAttck);
			}
		}
	}

	void ShowAoETargeting()
	{
		if (aoeType == AoEType.Directions)
		{
			foreach (Tile markedTile in markedTiles)
			{
				markedTile.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			markedTiles.Clear();
			markedTiles = GetAoETiles(currentAction.aoeType, currentAction.aoeRange, attackingCharacter);
			foreach (Tile t in markedTiles)
			{
				t.ShowOverlay(Tile.OverlayType.PossibleAttck);
			}
			if (targetedTiles.Count > 0)
			{
				targetedTiles[0].ShowOverlay(Tile.OverlayType.Selected);
			}
		}
		else
		{
			foreach (Tile targetedTile in targetedTiles)
			{
				targetedTile.HideOverlay(Tile.OverlayType.Selected);
			}
			targetedTiles.Clear();

			targetedTiles = GetAoETiles(currentAction.aoeType, currentAction.aoeRange, attackingCharacter);
			foreach (Tile t in targetedTiles)
			{
				t.ShowOverlay(Tile.OverlayType.Selected);
			}
		}
	}

	public class AccuracyResult
	{
		public int max;
		public List<int> rolls;
	}

	AccuracyResult GetMaximumAccuracy(int count)
	{
		List<int> rolls = new List<int>();
		for (int i = 0; i < Mathf.Abs(count); ++i)
		{
			rolls.Add(Util.RollDice(1, 6));
		}
		rolls.Sort();
		int maxValue = rolls.Count > 0 ? rolls[rolls.Count - 1] : 0;
		AccuracyResult result = new AccuracyResult();
		result.rolls = rolls;
		result.max = count < 0 ? -maxValue : maxValue;
		return result;
	}

	List<Tile> GetAoETiles(ActionPattern.AoEType aoeType, int aoeRange, Character c)
	{
		if (aoeType == ActionPattern.AoEType.Cone)
		{
			List<Tile> originTiles = TileGrid.Instance.FindCharacter(c);

			// Detect which tile the mouse is closest to
			Vector3 mouseWorld = GetWorldSpaceMouseOnGroundPlane();
			Tile closestOriginTile = originTiles[0];
			float minDist = float.MaxValue;
			foreach (Tile t in originTiles)
			{
				float dist = (t.transform.position - mouseWorld).sqrMagnitude;
				if (dist < minDist)
				{
					minDist = dist;
					closestOriginTile = t;
				}
			}

			Direction dir = GetFacingFromMousePosition(closestOriginTile.transform.position);
			return GetConeTiles(dir, aoeRange, closestOriginTile);
		}
		else if (aoeType == ActionPattern.AoEType.Line)
		{
			if (lastMousedOverTile == null)
			{
				return new List<Tile>();
			}
			List<Tile> outTiles = new List<Tile>();
			if (lastMousedOverTile != null)
			{
				if (Input.GetKeyDown(KeyCode.Z))
				{
					Debug.Log("1111");
				}
				Tile startingTile = TileGrid.Instance.FindCharacter(attackingCharacter)[0];
				outTiles = TileGrid.Instance.GetLineTilesTillCollision(startingTile, lastMousedOverTile);
			}
			return outTiles;
		}
		else if (aoeType == ActionPattern.AoEType.Burst)
		{
			if (lastMousedOverTile != null)
			{
				return TileGrid.Instance.GetAllTilesInRange(lastMousedOverTile, aoeRange);
			}
		}
		else if (aoeType == AoEType.Directions)
		{
			List<Tile> tiles = TileGrid.Instance.GetAllAdjacentTilesToCharacter(currentAction.forcedTarget);
			List<Tile> occupiedTiles = TileGrid.Instance.FindCharacter(currentAction.forcedTarget);

			tiles.RemoveAll(t => occupiedTiles.Contains(t));
			tiles.RemoveAll(t => t.character != null);

			return tiles;
		}
		return new List<Tile>();
	}

	void AddTileIfNotNull(List<Tile> tiles, Vector2Int pos)
	{
		Tile t = TileGrid.Instance.GetTile(pos.x, pos.y);
		if (t != null)
		{
			tiles.Add(t);
		}
	}

	static public Vector3 GetWorldSpaceMouseOnGroundPlane()
	{
		Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

		Plane p = new Plane(Vector3.up, 0);
		float enter = 0.0f;
		p.Raycast(r, out enter);

		return r.GetPoint(enter);
	}

	public static Direction GetFacingFromMousePosition(Vector3 position)
	{
		Vector3[] vecs = new Vector3[4];
		vecs[0] = position + new Vector3(0, 0, 1.5f);
		vecs[1] = position - new Vector3(0, 0, 1.5f);
		vecs[2] = position + new Vector3(1.5f, 0, 0);
		vecs[3] = position - new Vector3(1.5f, 0, 0);

		float minDistance = 99999;
		int index = 0;

		Vector3 hitPoint = GetWorldSpaceMouseOnGroundPlane();



		for (int i = 0; i < 4; ++i)
		{
			float distance = (vecs[i] - hitPoint).magnitude;
			if (distance < minDistance)
			{
				index = i;
				minDistance = distance;
			}
		}

		if (index == 0)
		{
			return Direction.North;
		}
		else if (index == 1)
		{
			return Direction.South;
		}
		else if (index == 2)
		{
			return Direction.East;
		}
		else
		{
			return Direction.West;
		}
	}

	public List<Tile> GetConeTiles(Direction dir, int aoeRange, Tile origin)
	{
		Vector2Int originVec = new Vector2Int(origin.x, origin.y);
		Vector2Int majorAxis = new Vector2Int(0, 0);
		Vector2Int minorAxis = new Vector2Int(0, 0);

		List<Tile> outTiles = new List<Tile>();

		if (dir == Direction.East || dir == Direction.West)
		{
			if (dir == Direction.East)
			{
				majorAxis.x = 1;
			}
			else
			{
				majorAxis.x = -1;
			}
			majorAxis.y = 0;
			minorAxis.x = 0;
			minorAxis.y = 1;
		}
		else
		{
			if (dir == Direction.North)
			{
				majorAxis.y = 1;
			}
			else
			{
				majorAxis.y = -1;
			}
			majorAxis.x = 0;
			minorAxis.x = 1;
			minorAxis.y = 0;
		}

		for (int i = 0; i < aoeRange; ++i)
		{
			int stepIndex = i + 1;
			Vector2Int finalPos = originVec + majorAxis * stepIndex;
			AddTileIfNotNull(outTiles, finalPos);
			for (int j = 0; j < stepIndex; ++j)
			{
				Vector2Int minorAxisPosition1 = finalPos + minorAxis * (j + 1);
				Vector2Int minorAxisPosition2 = finalPos - minorAxis * (j + 1);
				AddTileIfNotNull(outTiles, minorAxisPosition1);
				AddTileIfNotNull(outTiles, minorAxisPosition2);
			}
		}

		return outTiles;
	}
}
