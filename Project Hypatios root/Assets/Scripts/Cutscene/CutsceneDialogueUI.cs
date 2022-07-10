using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CutsceneDialogueUI : MonoBehaviour
{

    public Text text_DialogueContent;
    public Text text_SpeakerName;
    public Image portrait_Left;
    public Image portrait_Right;
    public GameObject continueButton;
    public float secondsPerChar = 0.1f;
    public AudioSource audio_typeWriter;
    [FoldoutGroup("References")] public Color color_inactiveSpeaker;
    [FoldoutGroup("References")] public Color color_activeSpeaker;
    [FoldoutGroup("Conversation")] public DialogSpeaker leftSpeaker;
    [FoldoutGroup("Conversation")] public DialogSpeaker rightSpeaker;
    [FoldoutGroup("Conversation")] public string dialogText;
    [FoldoutGroup("Conversation")] public bool allowContinue = false;


    [TextArea(3,4)]
    [FoldoutGroup("DEBUG")] public string DEBUG_TextToType = "It would be an interesting proposition.";

    private CutsceneDialogCache currentDialogue;
    private IEnumerator currentCoroutine;

    [FoldoutGroup("DEBUG")] [Button("Type Debug")]
    public void Debug_TestText()
    {
        TypeThisDialogue(DEBUG_TextToType);
    }

    private void Update()
    {
        if (allowContinue)
        {
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    #region Open Functions
    public void NewConversation()
    {
        portrait_Left.enabled = false;
        portrait_Right.enabled = false;
    }

    public void SkipDialogue()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        text_DialogueContent.text = dialogText;

    }

    public void ShowPortrait(DialogSpeaker dialogSpeaker, PortraitSpeaker portrait, bool isLeft = false)
    {
        if (isLeft)
        {
            leftSpeaker = dialogSpeaker;
            portrait_Left.sprite = portrait.portraitSprite;
            portrait_Left.enabled = true;
        }
        else
        {
            rightSpeaker = dialogSpeaker;
            portrait_Right.sprite = portrait.portraitSprite;
            portrait_Right.enabled = true;
        }
    }

    public void HidePortrait(DialogSpeaker dialogSpeaker)
    {
        if (leftSpeaker != null)
        {
            if (leftSpeaker.name == dialogSpeaker.name)
            {
                portrait_Left.enabled = false;
            }
        }

        if (rightSpeaker != null)
        {
            if (rightSpeaker.name == dialogSpeaker.name)
            {
                portrait_Right.enabled = false;
            }
        }
    }

    public void DisplayDialog(CutsceneDialogCache dialogEntry)
    {
        TypeThisDialogue(dialogEntry.dialogue);
        text_SpeakerName.text = dialogEntry.speakerName;
        dialogEntry.dialogEvent?.Invoke();

        bool greyLeftPortrait = true;
        bool greyRightPortrait = true;

        if (leftSpeaker != null)
            if (leftSpeaker.name == dialogEntry.speakerName)
                greyLeftPortrait = false;

        if (rightSpeaker != null)
            if (rightSpeaker.name == dialogEntry.speakerName)
                greyRightPortrait = false;

        if (greyLeftPortrait)
            portrait_Left.color = color_inactiveSpeaker;
        else portrait_Left.color = color_activeSpeaker;

        if (greyRightPortrait)
            portrait_Right.color = color_inactiveSpeaker;
        else portrait_Right.color = color_activeSpeaker;
    }

    #endregion

    private void TypeThisDialogue(string text)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentCoroutine = Typewriter(text);
        allowContinue = false;
        StartCoroutine(currentCoroutine);
    }

    IEnumerator Typewriter(string text)
    {
        text_DialogueContent.text = "";
        dialogText = text;

        var waitTimer = new WaitForSeconds(secondsPerChar);
        foreach (char c in dialogText)
        {
            text_DialogueContent.text = text_DialogueContent.text + c;
            if (audio_typeWriter != null) audio_typeWriter.Play();
            yield return waitTimer;
        }

        allowContinue = true;
    }
}
