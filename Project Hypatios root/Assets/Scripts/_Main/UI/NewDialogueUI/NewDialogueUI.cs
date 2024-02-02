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
    public NewDialogueButton button_HotkeyRespond;
    public NewDialogueButton button_ResponseSelection;
    public RectTransform pivot_Content;
    public Slider slider_DialogTimer;
    public float limitSizeContent = 300f;
    public float chatboxPos = 164f;
    public DialogSpeaker Speaker_ALDRICH;
    [SerializeField] private Interact_MultiDialoguesTrigger currentMultiDialogue;
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
    public Animator portraitAnimator;


    [SerializeField] [ReadOnly] private List<NewDialogueButton> allDialogueButtons = new List<NewDialogueButton>();
    [SerializeField] [ReadOnly] private int indexConversation = 0;
    [SerializeField] [ReadOnly] private bool isAwaitingResponse = false;
    [SerializeField] private List<DialogCommandEntry> _currentCommandEntries = new List<DialogCommandEntry>(); //this is all command entry
    [SerializeField] private List<DialogueSpeechCache> allDialogueHistory = new List<DialogueSpeechCache>(); //this only contain conversation messages
    private float timer = -1f;
    private bool isClosed = true;

    public List<DialogueSpeechCache> AllDialogueHistory { get => allDialogueHistory; }


    private DialogCommandEntry currentCommandEntry
    {
        get
        {
            //index: 0
            //count: 1
            if (indexConversation >= _currentCommandEntries.Count)
            {
                return null;
            }
            else return _currentCommandEntries[indexConversation];
        }
    }

    private void Start()
    {
        button_MessageText.gameObject.EnableGameobject(false);
        timer = 0.1f;
        button_HotkeyRespond.gameObject.EnableGameobject(false);
        button_ResponseSelection.gameObject.EnableGameobject(false);
    }


    [FoldoutGroup("DEBUG")]
    [Button("Skip Conversation")]
    public void SkipConversation()
    {
        timer = 0f;
        foreach (var entry in GetAllCommand_Message(DialogCommandEntry.Type.Message))
        {
            entry.dialogueCache.timer1 = 0.25f;
        }
    }

    public List<DialogCommandEntry> GetAllCommand_Message(DialogCommandEntry.Type _type)
    {
        return _currentCommandEntries.FindAll(x => x.commandType == _type);
    }

    [FoldoutGroup("DEBUG")]

    [Button("Debug Enqueue Dialogue")]
    public void Test1()
    {
        QueueDialogue(new DialogueSpeechCache(DEBUG_Dialogue, DEBUG_PortraitSpeaker.speaker.name, timer1: DEBUG_Timer, _videoClip: DEBUG_PortraitSpeaker.portraitVideo));
    }

    [SerializeField] [ReadOnly] private bool _hasInitiatedConversation = false;

    private void Update()
    {
        bool isStillTalking = false;
        isAwaitingResponse = IsAwaitingResponse();

        if (currentCommandEntry != null)
        {
            if (currentCommandEntry.commandType == DialogCommandEntry.Type.Message && timer > 0)
            {
                isStillTalking = true;
            }

            if (indexConversation == 0 && timer <= 0f && _hasInitiatedConversation == false)
            {
                GenerateNewButton();
                _hasInitiatedConversation = true;
                return;
            }
            else if (isStillTalking)
            {
                slider_DialogTimer.value = timer;
                timer -= Time.deltaTime;
                isClosed = false;
                CheckMessageToDisappear();
            }
            else if (IsAwaitingResponse())
            {
                Handle_ResponseSelection();
            }
            else if (_currentCommandEntries.Count > 0 && indexConversation < _currentCommandEntries.Count)
            {
                int projectedIndex = indexConversation + 1;
                if (projectedIndex >= _currentCommandEntries.Count)
                {
                    EndConversation();
                    _hasInitiatedConversation = false;
                    return;
                }
                else
                {
                    GoToNextEntry();
                }
            }

        }





        if (HasReachedLimit() && isStillTalking == false)
        {
            EndConversation();
            _hasInitiatedConversation = false;
        }
        else
        {
            dialogueAnimator.SetBool("Close", false);
            portraitAnimator.SetBool("Close", false);
        }
    }

    #region UI

    private void CheckMessageToDisappear()
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

    #endregion


    #region Handle Dialogue Commands

    private void Handle_ResponseSelection()
    {
        if (currentCommandEntry.commandType == DialogCommandEntry.Type.ResponseSelection)
        {
            //there will be maximum of 9 selections
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                ResponseDialogue(0);
            }
            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                ResponseDialogue(1);
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                ResponseDialogue(2);
            }
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                ResponseDialogue(3);
            }
            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                ResponseDialogue(4);
            }
            if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                ResponseDialogue(5);
            }
            if (Input.GetKeyUp(KeyCode.Alpha7))
            {
                ResponseDialogue(6);
            }
            if (Input.GetKeyUp(KeyCode.Alpha8))
            {
                ResponseDialogue(7);
            }
        }
        else if (currentCommandEntry.commandType == DialogCommandEntry.Type.RespondHotkey)
        {
            //press 'Z'
            if (Hypatios.Input.Respond.triggered)
            {
                GoToNextEntry();
            }
        }
    }

    public void ResponseDialogue(int index)
    {
        if (currentCommandEntry.branches.Count <= index)
        {
            return;
        }

        var node = currentCommandEntry.branches[index];
        DialogueSpeechCache newDialogue = new DialogueSpeechCache(node.respondDialogue, Speaker_ALDRICH.name, 0.02f);

        bool isLast = false;

        int projectedIndex = indexConversation + 1;
        if (projectedIndex >= _currentCommandEntries.Count)
        {
            newDialogue.timer1 = 3f;
            isLast = true;
        }

        //generate cache
        //Send_Message(newDialogue);


        if (node.isContinuing && isLast == false)
        {
            InsertDialogue(newDialogue, indexConversation + 1);
            GoToNextEntry();
        }
        else if (node.isContinuing && isLast == true)
        {
            QueueDialogue(newDialogue);
            GoToNextEntry();
        }
        else if (node.isContinuing == false)
        {
            WipeAllEntry();

            InitiateConversation(node.newConversation, true);
        }
        //still need to implement the branching, new dialogue


    }

    private bool HasReachedLimit()
    {
        if (_currentCommandEntries.Count == 0)
        {
            return true;
        }
        if (indexConversation >= _currentCommandEntries.Count)
        {
            return true;
        }
        if (currentCommandEntry == null)
        {
            return true;
        }

        return false;
    }


    private bool IsAwaitingResponse()
    {
        if (currentCommandEntry == null)
        {
            return false;
        }

        if (currentCommandEntry.commandType == DialogCommandEntry.Type.RespondHotkey |
            currentCommandEntry.commandType == DialogCommandEntry.Type.ResponseSelection)
        {
            return true;
        }

        return false;
    }


    [FoldoutGroup("DEBUG")]
    [Button("Next Entry")]
    private void GoToNextEntry()
    {
        indexConversation++;
        timer = -1f;
        GenerateNewButton();
    }

    private void BacktrackEntry()
    {
        indexConversation--;
        GenerateNewButton();
    }

    private void GenerateNewButton()
    {
        var entryCommand = _currentCommandEntries[indexConversation];

        if (entryCommand.commandType == DialogCommandEntry.Type.Message)
        {
            KillResponseButton();
            Receive_Message(entryCommand.dialogueCache);
        }
        else if (entryCommand.commandType == DialogCommandEntry.Type.RespondHotkey)
        {
            CreateButton_Hotkey();
        }
        else if (entryCommand.commandType == DialogCommandEntry.Type.ResponseSelection)
        {
            KillHotkeyButton();
            CreateButton_Responses(entryCommand);
        }
    }

    private void KillHotkeyButton()
    {
        var allHotkeyButtons = allDialogueButtons.FindAll(x => x.type == DialogCommandEntry.Type.RespondHotkey);
        foreach(var button in allHotkeyButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        allDialogueButtons.RemoveAll(x => x == null);
    }

    private void KillResponseButton()
    {
        var allButtons = allDialogueButtons.FindAll(x => x.type == DialogCommandEntry.Type.ResponseSelection);
        foreach (var button in allButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        allDialogueButtons.RemoveAll(x => x == null);
    }
    #endregion

    #region Generate Buttons

    private void Receive_Message(DialogueSpeechCache dialogueSpeech)
    {

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

        //dont bother with Aldrich
        if (dialogueSpeech.speakerName != Speaker_ALDRICH.name)
        {
            label_PortraitSpeakerName.text = $"{dialogueSpeech.speakerName.ToUpper()}";

            if (dialogueSpeech.videoClip != null)
            {
                ShowPortrait();
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

        }

        AllDialogueHistory.Add(dialogueSpeech);

    }

    private void CreateButton_Hotkey()
    {
        var buttonPrefab = Instantiate(button_HotkeyRespond, pivot_Content);

        buttonPrefab.gameObject.SetActive(true);
        allDialogueButtons.Add(buttonPrefab);

    }

    private void CreateButton_Responses(DialogCommandEntry dialogCommandEntry)
    {
        string s = "<b>RESPONSES</b>\n";
        var buttonPrefab = Instantiate(button_ResponseSelection, pivot_Content);

        buttonPrefab.gameObject.SetActive(true);
        allDialogueButtons.Add(buttonPrefab);

        int index = 0;
        int maxCount = dialogCommandEntry.branches.Count;

        foreach(var respond in dialogCommandEntry.branches)
        {
            s += $"[{index+1}] {respond.respondDialogue}";
            index++;
            if (index < maxCount)
            {
                s += "\n";
            }
        }

        buttonPrefab.text_ResponseSelect.text = s;

    }

    //This is Aldrich's POV, main differences:
    //No audio
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dialogueSpeech"></param>
    private void Send_Message(DialogueSpeechCache dialogueSpeech)
    {

        var buttonPrefab = Instantiate(button_MessageText, pivot_Content);
        string s = $"<b>{dialogueSpeech.speakerName}</b>: {dialogueSpeech.dialogue}";

        buttonPrefab.gameObject.SetActive(true);
        buttonPrefab.text_Message.text = s;
        timer = dialogueSpeech.timer1;
        dialogueSpeech.dialogEvent?.Invoke();

        slider_DialogTimer.maxValue = dialogueSpeech.timer1;
        allDialogueButtons.Add(buttonPrefab);

        AllDialogueHistory.Add(dialogueSpeech);
    }


    #endregion

    #region Public Commands
    public bool IsTalking()
    {
        if (_currentCommandEntries.Count == 0)
            return false;

        return true;
    }

    public void HidePortrait()
    {
        speakerVideoFeed.gameObject.SetActive(false);
        dialogueChatbox.anchoredPosition = new Vector2(0f,0f);
    }

    public void ShowPortrait()
    {
        speakerVideoFeed.gameObject.SetActive(true);
        dialogueChatbox.anchoredPosition = new Vector2(chatboxPos, 0f);

    }

    public void ResetConversation()
    {
        indexConversation = 0;
        timer = -1f;
        _hasInitiatedConversation = false;
        HidePortrait();
    }



    public void InitiateConversation(Interact_MultiDialoguesTrigger _newConverse, bool overrideAny = false)
    {
        currentMultiDialogue = _newConverse;

        if (overrideAny)
        {
            ResetConversation();
            WipeAllEntry();
        }

        //find all and queue all
        int index = 0;
        foreach (var dialogue in _newConverse.allDialogueActions)
        {
            var midss = dialogue as MultiDialogues_SingleSpeech;
            var mdrs = dialogue as MultiDialogues_ResponseSelection;

            if (midss != null)
            {
                var cache = dialogue.CollectEntry().dialogueCache;

                //dont generate response if the player speaks first
                if (cache.speakerName == Speaker_ALDRICH.name && index > 0)
                {
                    QueueHotkey();
                    QueueResponse(midss);
                }
                else
                {
                    QueueDialogue(cache);
                }
            }
            else if (mdrs != null)
            {
                QueueHotkey();
                QueueResponse(mdrs);
            }

            index++;
        }

    }


    public void InsertDialogue(DialogueSpeechCache dialogueSpeechCache, int index)
    {
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.Message);

        DialogueSpeechCache dialogue1 = dialogueSpeechCache;
        newEntry.dialogueCache = dialogue1;

        _currentCommandEntries.Insert(index, newEntry);
    }


    /// <summary>
    /// This can be directly injected from Interactables which loses most of its functions (like branching dialogues, responses, etc).
    /// To have full functionalities, use Interact_MultiDialoguesTrigger and InitiateConversation function.
    /// </summary>

    public void QueueDialogue(DialogueSpeechCache dialogueSpeechCache, bool dontQueue = false)
    {
        if (dontQueue && IsTalking())
        {
            return;
        }

        //create new entry
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.Message);

        DialogueSpeechCache dialogue1 = dialogueSpeechCache;
        newEntry.dialogueCache = dialogue1;

        EnqueueEntry(newEntry);
    }

    /// <summary>
    /// FOR NOW: This only generate single response
    /// </summary>
    /// <param name="node"></param>
    public void QueueResponse(MultiDialogues_SingleSpeech node)
    {
        //IF single speech, continue the conversation.
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.ResponseSelection);

        newEntry.branches.Add(new DialogCommandEntry.Branch(isContinuing: true, respondDialogue: node.Dialogue_Content, newConversation: null));
        EnqueueEntry(newEntry);

    }

    /// <summary>
    /// This is for multiple response
    /// </summary>
    /// <param name="node"></param>
    public void QueueResponse(MultiDialogues_ResponseSelection node)
    {
        //IF single speech, continue the conversation.
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.ResponseSelection);

        newEntry.branches = node.branches;
        EnqueueEntry(newEntry);

    }

    public void QueueHotkey()
    {
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.RespondHotkey);
        EnqueueEntry(newEntry);

    }

    #endregion

    private void EnqueueEntry(DialogCommandEntry commandEntry)
    {
        _currentCommandEntries.Add(commandEntry);
    }

    private void WipeAllEntry()
    {
        _currentCommandEntries.Clear();
    }



    private void EndConversation()
    {
        dialogueAnimator.SetBool("Close", true);
        portraitAnimator.SetBool("Close", true);

        _currentCommandEntries.Clear();

        foreach (var button in allDialogueButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        allDialogueButtons.Clear();

        isClosed = true;
        ResetConversation();
    }



}
