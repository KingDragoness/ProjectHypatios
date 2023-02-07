using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class DeadDialogue : MonoBehaviour
{

	public DeadDialogueButton prefab;
	public RectTransform rectTransform;
	public bool deleteByReverse = false;

	[FoldoutGroup("Debug")] public string testText = "Wew test.";

	[Tooltip("Height & space also included.")]
	public int gridLayoutSpace_Y = 40;

	public int MaximumMessageTotal = 8;
	public const float PROMPT_SCROLL_SPEED = 30;

	public List<DeadDialogueButton> promptMessages = new List<DeadDialogueButton>();

	private static DeadDialogue instance;

	private void Awake()
	{
		instance = this;
	}

	[FoldoutGroup("Debug")]
	[Button("TestText1")]
	public void TestText(string testText1)
	{
		PromptNotifyMessage(testText1, 9999f);
	}

	[FoldoutGroup("Debug")]
	[Button("TestText2")]
	public void TestText2(string testText1)
	{
		PromptNotifyMessage_Mod(testText1, 3f);
	}


	public static void PromptNotifyMessage_Mod(string text, float time)
	{
		instance.RefreshPrompt();
		DeadDialogueButton newPrompt = instance.CreateMessageButton();
		instance.promptMessages.Insert(0, newPrompt);
		newPrompt.SetMessage(text, time);
	}


	public static void PromptNotifyMessage(string text, float bonusTime)
	{
		instance.RefreshPrompt();
		DeadDialogueButton newPrompt = instance.CreateMessageButton();
		instance.promptMessages.Insert(0, newPrompt);
		newPrompt.AddBonusTimer(bonusTime);
		newPrompt.SetMessage(text);
	}

	private DeadDialogueButton CreateMessageButton()
	{
		var prefabInstant = Instantiate(prefab);
		prefabInstant.gameObject.SetActive(true);
		var prefabRT = prefabInstant.GetComponent<RectTransform>();

		prefabInstant.transform.SetParent(rectTransform);
		prefabRT.localScale = Vector3.one;
		prefabRT.localPosition = new Vector3(0, 0, 0);
		prefabRT.anchoredPosition = new Vector3(0, 0, 0);
		prefabRT.sizeDelta = prefab.GetRectTransform().sizeDelta;

		return prefabInstant;
	}

	private void RefreshPrompt()
	{
		promptMessages.RemoveAll(x => x == null);

		if (promptMessages.Count > MaximumMessageTotal)
		{
			var prompToKill = promptMessages[0];

			if (!deleteByReverse)
            {
				prompToKill = promptMessages[promptMessages.Count - 1];

				for(int x = MaximumMessageTotal - 1; x < promptMessages.Count; x++)
                {
					var message1 = promptMessages[x];
					message1.ForceKillPrompt();
                }
            }

			prompToKill.ForceKillPrompt();
		}
	}

	private void Update()
	{
		TweeningPrompts();
	}

	private void TweeningPrompts()
	{
		RefreshPrompt();

		int layoutIndex = 0;

		for (int x = 0; x < promptMessages.Count; x++)
		{
			var prompt = promptMessages[x];

			if (prompt == null)
			{
				continue;
			}

			var position_Y = layoutIndex * gridLayoutSpace_Y;
			var targetedPosition = new Vector2(0, -position_Y);
			var rectTransform = prompt.GetRectTransform();

			var targetedPositionV3 = new Vector3(targetedPosition.x, targetedPosition.y, 0);

			float step = (PROMPT_SCROLL_SPEED * Time.unscaledDeltaTime);
			step *= Mathf.Clamp(Vector3.Distance(targetedPositionV3, rectTransform.localPosition) * 0.1f, 1, 5); //Gets faster if delta getting further
			rectTransform.localPosition = Vector3.MoveTowards(rectTransform.localPosition, targetedPositionV3, step);

			//Sanity adjustment recttransform
			{
				rectTransform.localPosition = new Vector3(0, rectTransform.localPosition.y, 0);
			}

			layoutIndex++;
		}
	}
}
