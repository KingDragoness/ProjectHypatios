using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Sirenix.OdinInspector;

[System.Serializable]
public class CutsceneDialogCache
{
    public string dialogue, speakerName;
    public Sprite charPortrait = null;
    public AudioClip audioClip = null;
    public UnityEvent dialogEvent;

    public void CutsceneDialogueEntry(string _dialogue, string _speakerName, Sprite _charPortrait, AudioClip _audioClip, UnityEvent _dialogEvent)
    {
        dialogue = _dialogue;
        speakerName = _speakerName;
        charPortrait = _charPortrait;
        audioClip = _audioClip;
        dialogEvent = _dialogEvent;
    }

}

[System.Serializable]
public class DialogCommandEntry
{

    public enum Type
    {
        Message,
        Timer, //unused
        RespondHotkey,
        ResponseSelection
    }



    [System.Serializable]
    public class Branch
    {
        public bool isContinuing = false;
        public string respondDialogue = "What is this Machine of Madness thing?";
        public Interact_MultiDialoguesTrigger newConversation;

        public Branch(bool isContinuing, string respondDialogue, Interact_MultiDialoguesTrigger newConversation)
        {
            this.isContinuing = isContinuing;
            this.respondDialogue = respondDialogue;
            this.newConversation = newConversation;
        }
    }

    public Type commandType;
    public DialogueSpeechCache dialogueCache;
    [InfoBox("The game has been hard-coded to be able to have 8 responses only. Anything above 8 will not be able to press corresponding key to respond.", InfoMessageType.Warning, nameof(IsTooManyBranches))]
    public List<Branch> branches = new List<Branch>();

    public bool IsTooManyBranches => branches.Count >= 9;

    public DialogCommandEntry(Type commandType)
    {
        this.commandType = commandType;
    }
}

[System.Serializable]
public class DialogueSpeechCache
{

    public string dialogue, speakerName;
    public float timer1;
    public int ID = 0;
    public Sprite charPortrait = null;
    public VideoClip videoClip;
    public AudioClip audioClip = null;
    public UnityEvent dialogEvent;
    public DialogSpeaker dialogSpeakerAsset;

    public DialogueSpeechCache(string dialogue, string speakerName, float timer1, Sprite charPortrait = null, AudioClip audioClip = null, UnityEvent _dialogEvent = null, VideoClip _videoClip = null, DialogSpeaker _dialogSpeakerAsset = null)
    {
        this.dialogue = dialogue;
        this.speakerName = speakerName;
        this.timer1 = timer1;
        this.charPortrait = charPortrait;
        this.audioClip = audioClip;
        this.dialogEvent = _dialogEvent;
        this.ID = Hypatios.TimeTick;
        this.videoClip = _videoClip;
        this.dialogSpeakerAsset = _dialogSpeakerAsset;
    }

    public DialogueSpeechCache(DialogueSpeechCache origin)
    {
        this.dialogue = origin.dialogue;
        this.speakerName = origin.speakerName;
        this.timer1 = origin.timer1;
        this.charPortrait = origin.charPortrait;
        this.audioClip = origin.audioClip;
        this.dialogEvent = origin.dialogEvent;
        this.ID = Hypatios.TimeTick;
        this.videoClip = origin.videoClip;
        this.dialogSpeakerAsset = origin.dialogSpeakerAsset;
    }
}
