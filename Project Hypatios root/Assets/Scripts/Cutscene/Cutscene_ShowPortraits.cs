using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Cutscene_ShowPortraits : CutsceneAction
{
    public DialogSpeaker dialogSpeaker;
    public PortraitSpeaker portraitSpeaker;
    public bool isLeftSide = false;
    public bool hidePortrait = false;


    public override void ExecuteAction()
    {

        //Cutscene UI and display portrait
        var cutsceneUI = MainUI.Instance.cutsceneUI;
        if (!hidePortrait)
        {
            cutsceneUI.ShowPortrait(dialogSpeaker, portraitSpeaker, isLeftSide);
        }
        else
        {
            cutsceneUI.HidePortrait(dialogSpeaker);
        }
        parentCutscene.NextActionEntry();
    }

    public override void OnDone()
    {

        base.OnDone();
    }
}
