﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Used by the tooltip prompt UI
/// </summary>
public class DeadDialogueButton : MonoBehaviour
{

	public Text messageText;
	public CanvasGroup canvasGroup;

	public const float TIMER_PROMPT_DISAPPEAR = 10;
	private float currentTimer = 1;
	private float additionalBonusTimer = 0;

	public void AddBonusTimer(float time)
	{
		additionalBonusTimer = Mathf.Clamp(time, 0, 100);
	}

	public void SetMessage(string s)
	{
		messageText.text = s;
		currentTimer = TIMER_PROMPT_DISAPPEAR + additionalBonusTimer;
	}

	public void SetMessage(string s, float time)
	{
		messageText.text = s;
		currentTimer = time;
	}

	public RectTransform GetRectTransform()
	{
		return GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (currentTimer < 0)
		{
			DestroyPrompt();
		}
		else
		{
			currentTimer -= Time.unscaledDeltaTime;

			if (currentTimer > 1 && canvasGroup.alpha < 1)
            {
				canvasGroup.alpha += Time.unscaledDeltaTime;
			}

			if (currentTimer < 1 && canvasGroup.alpha > 0)
			{
				canvasGroup.alpha -= Time.unscaledDeltaTime;
			}
		}
	}

	public void ForceKillPrompt()
	{
		currentTimer = 1;
	}

	private void DestroyPrompt()
	{
		Destroy(gameObject);
	}

}
