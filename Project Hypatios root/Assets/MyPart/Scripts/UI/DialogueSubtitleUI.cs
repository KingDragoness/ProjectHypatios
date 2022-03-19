using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSubtitleUI : MonoBehaviour
{

    [TextArea(3,4)]
    public string DEBUG_Dialogue;
    public string DEBUG_SpeakerName;
    public float DEBUG_Timer = 4;
    public Sprite emptySprite;
    [Space]
    public Animator dialogueAnimator;
    public Text Label_DialogueContent;
    public Text Label_SpeakerName;
    public Image Image_SpeakerPortrait;
    public AudioSource audioSource;

    public static DialogueSubtitleUI instance;

    private Queue<DialogueSpeech> dialogueSpeeches = new Queue<DialogueSpeech>();
    private float timer = 2f;
    private bool isClosed = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Image_SpeakerPortrait.sprite = emptySprite;
        Close();
        timer = 0f;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            isClosed = false;
        }
        else
        {
            if (isClosed && dialogueSpeeches.Count > 0)
            {
                DisplayThisDialogue(dialogueSpeeches.Peek());
                return;
            }

            if (dialogueSpeeches.Count > 0)
            {
                dialogueSpeeches.Dequeue();
                Debug.Log("test dequeue");
                if (dialogueSpeeches.Count > 0) DisplayThisDialogue(dialogueSpeeches.Peek());
            }
            else
            {
                Close();
            }
        }
    }

    [ContextMenu("Test1")]
    public void Test1()
    {
        QueueDialogue(DEBUG_Dialogue, DEBUG_SpeakerName, DEBUG_Timer, true);
    }

    public void QueueDialogue(string dialogue, string speakerName, float timer1)
    {
        DialogueSpeech dialogue1 = new DialogueSpeech(dialogue, speakerName, timer1);
        OverrideDialogue(dialogue1);

    }

    public void QueueDialogue(string dialogue, string speakerName, float timer1, bool enqueueDialog)
    {
        DialogueSpeech dialogue1 = new DialogueSpeech(dialogue, speakerName, timer1);
        if (enqueueDialog) EnqueueDialogue(dialogue1); else OverrideDialogue(dialogue1);

    }

    public void QueueDialogue(string dialogue, string speakerName, float timer1, bool enqueueDialog, Sprite charPortrait = null, AudioClip audioClip = null)
    {
        DialogueSpeech dialogue1 = new DialogueSpeech(dialogue, speakerName, timer1, charPortrait, audioClip);
        if (enqueueDialog) EnqueueDialogue(dialogue1); else OverrideDialogue(dialogue1);

    }


    private void DisplayThisDialogue(DialogueSpeech dialogueSpeech)
    {
        dialogueAnimator.SetBool("Close", false);
        Label_DialogueContent.text = dialogueSpeech.dialogue;
        Label_SpeakerName.text = dialogueSpeech.speakerName;
        timer = dialogueSpeech.timer1;

        if (dialogueSpeech.charPortrait != null)
        {
            Image_SpeakerPortrait.sprite = dialogueSpeech.charPortrait;
        }
        else
        {
            Image_SpeakerPortrait.sprite = emptySprite;
        }

        if (dialogueSpeech.audioClip != null)
        {
            audioSource.clip = dialogueSpeech.audioClip;
            audioSource.Play();
        }
    }

    private void EnqueueDialogue(DialogueSpeech dialogueSpeech)
    {
        dialogueSpeeches.Enqueue(dialogueSpeech);
    }

    private void OverrideDialogue(DialogueSpeech dialogueSpeech)
    {
        dialogueSpeeches.Clear();
        dialogueSpeeches.Enqueue(dialogueSpeech);
        DisplayThisDialogue(dialogueSpeech);
    }



    public void Close()
    {
        dialogueAnimator.SetBool("Close", true);
        isClosed = true;
    }

}
