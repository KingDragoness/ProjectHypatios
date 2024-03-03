using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

//Queue actions
//MultiDialogues_SingleSpeech = Show message
//MultiDialogues_Response = show responses (lead to other Interact_multidialoguestrigger

public abstract class MultiDialogue_Action : MonoBehaviour
{

    public Interact_MultiDialoguesTrigger parentMultiDialogues;

    public virtual void Start()
    {
        if (parentMultiDialogues == null) parentMultiDialogues = GetComponentInParent<Interact_MultiDialoguesTrigger>();
    }

    public abstract DialogCommandEntry CollectEntry();
    public virtual void OnDone()
    {
        //Send it to parent
    }
}


public class MultiDialogues_SingleSpeech : MultiDialogue_Action
{
    [TextArea(3, 4)]
    public string Dialogue_Content;
    public DialogSpeaker dialogSpeaker;
    public PortraitSpeaker portraitSpeaker;
    public AudioClip dialogAudioClip;
    public float Dialogue_Timer = 4;
    public UnityEvent OnDialogTriggered;

    public override DialogCommandEntry CollectEntry()
    {
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.Message);

        DialogueSpeechCache dialogue1 = new DialogueSpeechCache(Dialogue_Content,
            dialogSpeaker.name,
            Dialogue_Timer,
            portraitSpeaker.portraitSprite,
            audioClip: dialogAudioClip,
            _dialogEvent: OnDialogTriggered,
            _videoClip: portraitSpeaker.portraitVideo,
            _dialogSpeakerAsset: dialogSpeaker);
            newEntry.dialogueCache = dialogue1;

        return newEntry;


    }

    //just do it here.

}
