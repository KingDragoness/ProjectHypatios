using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueSpeechCache
{

    public string dialogue, speakerName;
    public float timer1;
    public bool isImportant = false;
    public int priority = 0;
    public Sprite charPortrait = null;
    public AudioClip audioClip = null;
    public UnityEvent dialogEvent;

    public DialogueSpeechCache(string dialogue, string speakerName, float timer1, Sprite charPortrait = null, AudioClip audioClip = null, int priority = 0
        , bool _isImportant = false, UnityEvent _dialogEvent = null)
    {
        this.dialogue = dialogue;
        this.speakerName = speakerName;
        this.timer1 = timer1;
        this.charPortrait = charPortrait;
        this.audioClip = audioClip;
        this.isImportant = _isImportant;
        this.priority = priority;
        this.dialogEvent = _dialogEvent;
    }
}
