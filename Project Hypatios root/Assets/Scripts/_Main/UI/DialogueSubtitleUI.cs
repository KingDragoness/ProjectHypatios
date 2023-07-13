using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;


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
    public Slider slider_DialogTimer;
    public AudioSource audioSource;

    public static DialogueSubtitleUI instance;

    [SerializeField] private List<DialogueSpeechCache> _currentDialogueList = new List<DialogueSpeechCache>();
    [SerializeField] private List<DialogueSpeechCache> allDialogueHistory = new List<DialogueSpeechCache>();
     private float timer = 2f;
    private bool isClosed = true;

    public List<DialogueSpeechCache> AllDialogueHistory { get => allDialogueHistory;  }

    public bool IsTalking()
    {
        if (_currentDialogueList.Count == 0)
            return false;

        return true;
    }

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
            slider_DialogTimer.value = timer;
            timer -= Time.deltaTime;
            isClosed = false;
        }
        else
        {
            if (isClosed && _currentDialogueList.Count > 0)
            {
                DisplayThisDialogue(_currentDialogueList[0]);
                return;
            }

            if (_currentDialogueList.Count > 0)
            {
                _currentDialogueList.RemoveAt(0);
                //Debug.Log($"test dequeue | Speech Left: [{dialogueSpeeches.Count}]");
                if (_currentDialogueList.Count > 0) DisplayThisDialogue(_currentDialogueList[0]);
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
        QueueDialogue(DEBUG_Dialogue, DEBUG_SpeakerName, DEBUG_Timer);
    }

    public void OpenTutorialDialogue(string dialogue, string speakerName)
    {
        dialogueAnimator.SetBool("Close", false);
        Label_DialogueContent.text = dialogue;
        Label_SpeakerName.text = speakerName;
        Image_SpeakerPortrait.sprite = emptySprite;

    }

    public void ExitTutorialDialogue()
    {
        Close();
    }

    //Full set
    public void QueueDialogue(string dialogue, string speakerName, float timer1, Sprite charPortrait = null,
        AudioClip audioClip = null, int priorityLevel = -1, bool isImportant = false, bool shouldOverride = false, UnityEvent entryEvent = null, int _ID = 0)
    {
        DialogueSpeechCache dialogue1 = new DialogueSpeechCache(dialogue, speakerName, timer1, charPortrait, audioClip, priorityLevel, isImportant, entryEvent, _ID);

        if (shouldOverride == false)
        {
            if (_currentDialogueList.Count != 0)
            {
                if (!_currentDialogueList[0].isImportant && dialogue1.isImportant)
                {
                    OverrideDialogue(dialogue1);
                }
                else if (_currentDialogueList[0].isImportant && !dialogue1.isImportant && dialogue1.priority < 0)
                {
                    //EnqueueDialogue(dialogue1);
                }
                else if (_currentDialogueList[0].isImportant)
                {
                    EnqueueDialogue(dialogue1);
                }
            }
            else
            {
                EnqueueDialogue(dialogue1);

            }
        }
        else
        {
            OverrideDialogue(dialogue1);
        }
    }

    public void ForceDisplay()
    {
        DisplayThisDialogue(_currentDialogueList[0]);
    }


    private void DisplayThisDialogue(DialogueSpeechCache dialogueSpeech)
    {
        dialogueAnimator.SetBool("Close", false);
        Label_DialogueContent.text = dialogueSpeech.dialogue;
        Label_SpeakerName.text = dialogueSpeech.speakerName;
        timer = dialogueSpeech.timer1;
        slider_DialogTimer.maxValue = timer;
        dialogueSpeech.dialogEvent?.Invoke();

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

        AllDialogueHistory.Add(dialogueSpeech);
    }

    private void EnqueueDialogue(DialogueSpeechCache dialogueSpeech)
    {
        _currentDialogueList.Add(dialogueSpeech);
    }

    private void OverrideDialogue(DialogueSpeechCache dialogueSpeech)
    {
        _currentDialogueList.RemoveAll(x => x.ID != dialogueSpeech.ID);
        _currentDialogueList.Add(dialogueSpeech);
        Debug.Log($"{dialogueSpeech.speakerName} {_currentDialogueList.Count}");
    }



    public void Close()
    {
        dialogueAnimator.SetBool("Close", true);
        isClosed = true;
    }

}
