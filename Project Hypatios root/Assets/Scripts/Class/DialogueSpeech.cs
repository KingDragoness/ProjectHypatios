using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueSpeech
{

    public string dialogue, speakerName;
    public float timer1;
    public Sprite charPortrait = null;
    public AudioClip audioClip = null;

    public DialogueSpeech(string dialogue, string speakerName, float timer1, Sprite charPortrait = null, AudioClip audioClip = null)
    {
        this.dialogue = dialogue;
        this.speakerName = speakerName;
        this.timer1 = timer1;
        this.charPortrait = charPortrait;
        this.audioClip = audioClip;
    }
}
