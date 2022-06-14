using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MultiDialogues_SingleSpeech : MonoBehaviour
{
    [TextArea(3, 4)]
    public string Dialogue_Content;
    public DialogSpeaker dialogSpeaker;
    public PortraitSpeaker portraitSpeaker;
    public AudioClip dialogAudioClip;
    //public bool _isImportant;
    public float Dialogue_Timer = 4;
}
