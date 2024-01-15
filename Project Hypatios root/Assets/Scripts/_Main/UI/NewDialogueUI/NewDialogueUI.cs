using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Video;

public class NewDialogueUI : MonoBehaviour
{

    [FoldoutGroup("DEBUG")] [TextArea(3, 4)] public string DEBUG_Dialogue;
    [FoldoutGroup("DEBUG")] public PortraitSpeaker DEBUG_PortraitSpeaker;
    [FoldoutGroup("DEBUG")] public float DEBUG_Timer = 4;

    public NewDialogueButton button_MessageText;
    public NewDialogueButton button_Timer;
    public RectTransform pivot_Content;
    public Slider slider_DialogTimer;
    public float limitSizeContent = 300f;
    [Space]
    [FoldoutGroup("Speakerfeed")] public GameObject speakerVideoFeed;
    [FoldoutGroup("Speakerfeed")] public GameObject videoFeed_NoiseTransition;
    [FoldoutGroup("Speakerfeed")] public Image image_SpeakerFeed;
    [FoldoutGroup("Speakerfeed")] public Text label_PortraitSpeakerName;
    [FoldoutGroup("Speakerfeed")] public VideoPlayer speakerFeedPlayer;
    [FoldoutGroup("Speakerfeed")] public VideoClip fallbackClip;
    [Space]
    public RectTransform dialogueChatbox;
    public AudioSource audioSource;
    public Animator dialogueAnimator;


    [SerializeField] [ReadOnly] private List<NewDialogueButton> allDialogueButtons = new List<NewDialogueButton>();

    [SerializeField] private List<DialogueSpeechCache> _currentDialogueList = new List<DialogueSpeechCache>();
    [SerializeField] private List<DialogueSpeechCache> allDialogueHistory = new List<DialogueSpeechCache>();
    private float timer = 2f;
    private bool isClosed = true;

    public List<DialogueSpeechCache> AllDialogueHistory { get => allDialogueHistory; }


    private void Start()
    {
        button_MessageText.gameObject.EnableGameobject(false);
        timer = 0.1f;
        //button_Timer.gameObject.EnableGameobject(false);
    }


    [FoldoutGroup("DEBUG")]
    [Button("Skip Conversation")]
    public void SkipConversation()
    {
        timer = 0f;
        foreach (var dialogue in _currentDialogueList)
        {
            dialogue.timer1 = 0.25f;
        }
    }

    [FoldoutGroup("DEBUG")]

    [Button("Debug Enqueue Dialogue")]
    public void Test1()
    {
        QueueDialogue(DEBUG_Dialogue, DEBUG_PortraitSpeaker.speaker.name, DEBUG_Timer, isImportant: true);
    }



    private void Update()
    {
        if (timer > 0)
        {
            slider_DialogTimer.value = timer;
            timer -= Time.deltaTime;
            isClosed = false;
            CheckMessageToDisappear();
        }
        else
        {
            bool notAllowNextDisplay = false;

            if (notAllowNextDisplay == false)
            {

                if (isClosed && _currentDialogueList.Count > 0)
                {
                    NewMessage(_currentDialogueList[0]);
                    return;
                }

                if (_currentDialogueList.Count > 0)
                {
                    _currentDialogueList.RemoveAt(0);
                    if (_currentDialogueList.Count > 0) NewMessage(_currentDialogueList[0]);
                }
            }

            if (_currentDialogueList.Count <= 0) Close();
        }
    }

    public void CheckMessageToDisappear()
    {
        float contentHeight = pivot_Content.sizeDelta.y;

        if (contentHeight >= limitSizeContent)
        {
            allDialogueButtons.RemoveAll(x => x == null);
            int index = allDialogueButtons.Count - 1;

            foreach (var button in allDialogueButtons)
            { 
                float yPos = Mathf.Abs(button.rectTransform.anchoredPosition.y);
                float delta = contentHeight - yPos;

                if (delta > limitSizeContent && index >= 1)
                {
                    button.WipeDeleteButton();
                }

                index--;
            }

        }
    }


    public void QueueDialogue(string dialogue, string speakerName, float timer1, Sprite charPortrait = null,
    AudioClip audioClip = null, int priorityLevel = -1, bool isImportant = false, bool shouldOverride = false, UnityEvent entryEvent = null, int _ID = 0, VideoClip _videoClip = null)
    {
        DialogueSpeechCache dialogue1 = new DialogueSpeechCache(dialogue, speakerName, timer1, charPortrait, audioClip, priorityLevel, isImportant, entryEvent, _ID, _videoClip);

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

        foreach(var button in allDialogueButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        allDialogueButtons.Clear();

        isClosed = true;
    }

    public void NewMessage(DialogueSpeechCache dialogueSpeech)
    {
        dialogueAnimator.SetBool("Close", false);

        var buttonPrefab = Instantiate(button_MessageText, pivot_Content);
        string s = $"<b>{dialogueSpeech.speakerName}</b>: {dialogueSpeech.dialogue}";

        buttonPrefab.gameObject.SetActive(true);
        buttonPrefab.text_Message.text = s;
        timer = dialogueSpeech.timer1;
        dialogueSpeech.dialogEvent?.Invoke();

        if (dialogueSpeech.audioClip != null)
        {
            audioSource.clip = dialogueSpeech.audioClip;
            audioSource.Play();
        }

        slider_DialogTimer.maxValue = dialogueSpeech.timer1;
        allDialogueButtons.Add(buttonPrefab);
        label_PortraitSpeakerName.text = $"{dialogueSpeech.speakerName.ToUpper()}";

        if (dialogueSpeech.videoClip != null)
        {
            image_SpeakerFeed.gameObject.EnableGameobject(true);

            if (speakerFeedPlayer.clip != dialogueSpeech.videoClip)
            {
                videoFeed_NoiseTransition.gameObject.EnableGameobject(true);
            }

            speakerFeedPlayer.clip = dialogueSpeech.videoClip;
            speakerFeedPlayer.Play();
        }
        else
        {
            if (speakerFeedPlayer.clip != dialogueSpeech.videoClip)
            {
                videoFeed_NoiseTransition.gameObject.EnableGameobject(true);
            }

            image_SpeakerFeed.gameObject.EnableGameobject(false);
        }

        AllDialogueHistory.Add(dialogueSpeech);

    }


}
