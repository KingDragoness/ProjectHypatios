using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Sirenix.OdinInspector;

public class Interact_MultiDialoguesTrigger : MonoBehaviour
{

    public List<MultiDialogue_Action> allDialogueActions;
    public List<Transform> ActivatingArea;
    public UnityEvent OnDialogueTriggered;
    public bool shouldOverride = false;
    public bool dontQueue = false;

    public Transform player;
    public bool AutoScanDialogues = false;
    public bool DEBUG_DrawGizmos = false;

    public bool ignoreTrigger = false;
    [SerializeField] private bool alreadyTriggered = false;

    void Start()
    {
        player = Hypatios.Player.transform;
        if (AutoScanDialogues)
            ScanDialogues();
    }

    [ContextMenu("Scan Dialogues")]
    public void ScanDialogues()
    {
        {
            allDialogueActions = GetComponentsInChildren<MultiDialogue_Action>().ToList();
        }
    }


    private void OnDrawGizmos()
    {
        if (DEBUG_DrawGizmos == false)
        {
            return;
        }

        foreach (Transform t in ActivatingArea)
        {
            if (t == null)
                continue;

            Gizmos.matrix = t.transform.localToWorldMatrix;
            Gizmos.color = new Color(0.05f, 0.8f, 0.05f, 0.1f);
            Gizmos.DrawWireCube(Vector3.zero, t.localScale * 0.5f );
        }

    }

    void FixedUpdate()
    {

        if (player == null)
        {
            return;
        }

        bool activate = false;

        foreach (var t in ActivatingArea)
        {
            activate = IsInsideOcclusionBox(t, player.position);

            if (activate)
                TriggerMessage();
        }
    }

    [Button("Trigger Manual")]
    public void TriggerMessage()
    {
        if (alreadyTriggered && ignoreTrigger == false) return;
        ScanDialogues();
        int ID1 = Random.Range(-9999, 9999);


        if (Hypatios.Game.DEBUG_UseNewDialogueSystem == false)
        {
            List<MultiDialogues_SingleSpeech> dialogueNodes = new List<MultiDialogues_SingleSpeech>();

            foreach (var i in allDialogueActions.FindAll(x => x is MultiDialogues_SingleSpeech))
            {
                dialogueNodes.Add(i as MultiDialogues_SingleSpeech);
            }

            foreach (var dialog in dialogueNodes)
            {
                Sprite portrait = null;
                VideoClip videoClip = null;

                if (dialog.portraitSpeaker == null)
                {
                    portrait = dialog.dialogSpeaker.defaultSprite;
                }
                else
                {
                    portrait = dialog.portraitSpeaker.portraitSprite;
                    videoClip = dialog.portraitSpeaker.portraitVideo;
                }

                Hypatios.Dialogue.QueueDialogue(dialog.Dialogue_Content,
                    dialog.dialogSpeaker.name,
                    dialog.Dialogue_Timer,
                    portrait,
                    dialog.dialogAudioClip,
                    shouldOverride: shouldOverride,
                    entryEvent: dialog.OnDialogTriggered,
                    _videoClip: videoClip);

            }
            if (shouldOverride) Hypatios.Dialogue.ForceDisplay();
        }
        else
        {
            if (Hypatios.NewDialogue.IsTalking() && dontQueue == true)
            {
                return;
            }

            Hypatios.NewDialogue.InitiateConversation(this, shouldOverride);
        }

        OnDialogueTriggered?.Invoke();
        alreadyTriggered = true;


    }

    public List<DialogueSpeechCache> GenerateCacheDialogues()
    {
        List<DialogueSpeechCache> allDialogues = new List<DialogueSpeechCache>();
        List<MultiDialogues_SingleSpeech> dialogueNodes = new List<MultiDialogues_SingleSpeech>();

        foreach (var i in allDialogueActions.FindAll(x => x is MultiDialogues_SingleSpeech))
        {
            dialogueNodes.Add(i as MultiDialogues_SingleSpeech);
        }

        foreach (var dialog in dialogueNodes)
        {
            Sprite portrait = null;
            VideoClip videoClip = null;

            if (dialog.portraitSpeaker == null)
            {
                portrait = dialog.dialogSpeaker.defaultSprite;
            }
            else
            {
                portrait = dialog.portraitSpeaker.portraitSprite;
                videoClip = dialog.portraitSpeaker.portraitVideo;
            }

            DialogueSpeechCache dialogue1 = new DialogueSpeechCache(dialog.Dialogue_Content,
                dialog.dialogSpeaker.name,
                dialog.Dialogue_Timer,
                portrait,
                dialog.dialogAudioClip,
                _dialogEvent: dialog.OnDialogTriggered,
                _videoClip: videoClip);

            allDialogues.Add(dialogue1);
        }

        return allDialogues;

    }


    [Button("Trigger from Prefab")]
    public void TriggerMessage_Prefab()
    {
        if (Application.isPlaying == false) return;

        var objectPrefab1 = Instantiate(this);
        objectPrefab1.TriggerMessage();
        Destroy(objectPrefab1, 1f);
    }


    public void ManualTriggeredCheck()
    {
        alreadyTriggered = true;
    }

    public static bool IsInsideOcclusionBox(Transform box, Vector3 aPoint)
    {
        Vector3 localPos = box.InverseTransformPoint(aPoint);

        if (Mathf.Abs(localPos.x) < (box.localScale.x / 2) && Mathf.Abs(localPos.y) < (box.localScale.y / 2) && Mathf.Abs(localPos.z) < (box.localScale.z / 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
