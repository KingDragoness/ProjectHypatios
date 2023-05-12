using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public class Cutscene_DialogEntry : CutsceneAction
{
    [TextArea(3, 4)]
    public string Dialogue_Content;
    public DialogSpeaker dialogSpeaker;
    public PortraitSpeaker portraitSpeaker;
    public AudioClip dialogAudioClip;
    public UnityEvent OnDialogTriggered;
    public UnityEvent OnDoneDialog;


    public override void ExecuteAction()
    {
        CutsceneDialogCache cache = new CutsceneDialogCache();
        cache.audioClip = dialogAudioClip;
        cache.dialogEvent = OnDialogTriggered;
        cache.dialogue = Dialogue_Content;
        cache.speakerName = dialogSpeaker.name;
        cache.charPortrait = portraitSpeaker.portraitSprite;

        //Cutscene UI and display this dialogue
        var cutsceneUI = MainUI.Instance.cutsceneUI;
        cutsceneUI.DisplayDialog(cache);
    }

    public override void OnDone()
    {
        OnDoneDialog?.Invoke();
        base.OnDone();
    }
}
