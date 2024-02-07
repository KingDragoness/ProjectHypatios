﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

public class CutsceneDialogueUI : MonoBehaviour
{

    public Text text_DialogueContent;
    public Text text_SpeakerName;
    public Image portrait_Left;
    public Image portrait_Right;
    public GameObject continueButton;
    public CinemachineBrain cutsceneCamera;
    public float secondsPerChar = 0.1f;
    public AudioSource audio_typeWriter;
    [FoldoutGroup("References")] public Color color_inactiveSpeaker;
    [FoldoutGroup("References")] public Color color_activeSpeaker;
    [FoldoutGroup("Conversation")] public DialogSpeaker leftSpeaker;
    [FoldoutGroup("Conversation")] public DialogSpeaker rightSpeaker;
    [FoldoutGroup("Conversation")] public string dialogText;
    [FoldoutGroup("Conversation")] public bool allowContinue = false;

    [FoldoutGroup("Speakerfeed")] public GameObject speakerVideoFeed;
    [FoldoutGroup("Speakerfeed")] public GameObject videoFeed_NoiseTransition;
    [FoldoutGroup("Speakerfeed")] public Image image_SpeakerFeed;
    [FoldoutGroup("Speakerfeed")] public VideoPlayer speakerFeedPlayer;
    [FoldoutGroup("Speakerfeed")] public VideoClip fallbackClip;
    public Animator portraitAnimator;


    [TextArea(3,4)]
    [FoldoutGroup("DEBUG")] public string DEBUG_TextToType = "It would be an interesting proposition.";

    private CutsceneDialogCache currentDialogue;
    private IEnumerator currentCoroutine;

    [FoldoutGroup("DEBUG")] [Button("Type Debug")]
    public void Debug_TestText()
    {
        TypeThisDialogue(DEBUG_TextToType);
    }

    private void Start()
    {
        ResetCutscene();
    }

    private void OnEnable()
    {
    }

    public void ResetCutscene()
    {
        portraitAnimator.SetBool("Close", true);
        speakerVideoFeed.gameObject.SetActive(false);
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
        //This is not used anymore
        if (isLeft)
        {
            leftSpeaker = dialogSpeaker;
            if (portrait != null) portrait_Left.sprite = portrait.portraitSprite; else portrait_Left.sprite = null;
            portrait_Left.enabled = true;
        }
        else
        {
            rightSpeaker = dialogSpeaker;
            if (portrait != null) portrait_Right.sprite = portrait.portraitSprite; else portrait_Right.sprite = null;
            portrait_Right.enabled = true;
        }

        if (portrait.portraitVideo != null)
        {
            speakerVideoFeed.gameObject.SetActive(true);
            portraitAnimator.SetBool("Close", false);

            image_SpeakerFeed.gameObject.EnableGameobject(true);

            if (speakerFeedPlayer.clip != portrait.portraitVideo)
            {
                videoFeed_NoiseTransition.gameObject.EnableGameobject(true);
            }

            speakerFeedPlayer.clip = portrait.portraitVideo;
            speakerFeedPlayer.Play();
        }
        else
        {


            if (speakerFeedPlayer.clip != portrait.portraitVideo)
            {
                videoFeed_NoiseTransition.gameObject.EnableGameobject(true);
            }

            image_SpeakerFeed.gameObject.EnableGameobject(false);
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
        var dialogueCustom = CustomAilment_Dialogue(dialogEntry);

        if (dialogueCustom != null)
        {
            TypeThisDialogue(dialogueCustom.dialogue);
        }
        else TypeThisDialogue(dialogEntry.dialogue);

        text_SpeakerName.text = dialogEntry.speakerName.ToUpper();
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

    private CutsceneDialogCache CustomAilment_Dialogue(CutsceneDialogCache originalDialogue)
    {
        CutsceneDialogCache dialogueSpeech = new CutsceneDialogCache();
        dialogueSpeech.CutsceneDialogueEntry(originalDialogue.dialogue, originalDialogue.speakerName, originalDialogue.charPortrait, originalDialogue.audioClip, originalDialogue.dialogEvent);

        bool anyAilment = false;

        if (Hypatios.Player.GetStatusEffectGroup(Hypatios.Dialogue.ailmentNoSpeak) && originalDialogue.speakerName == Hypatios.Dialogue.speaker.name)
        {
            dialogueSpeech.dialogue = "...";
        }
        else
        {
            return null;
        }

        return dialogueSpeech;
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
