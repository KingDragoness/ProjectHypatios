using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interact_MultiDialoguesTrigger : MonoBehaviour
{

    public List<MultiDialogues_SingleSpeech> allDialogues;
    public List<Transform> ActivatingArea;
    public UnityEvent OnDialogueTriggered;
    public bool shouldOverride = false;
    public bool isImportant = true;

    public Transform player;
    public bool AutoScanDialogues = false;
    public bool DEBUG_DrawGizmos = false;

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
            allDialogues = GetComponentsInChildren<MultiDialogues_SingleSpeech>().ToList();
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

    [ContextMenu("Trigger Message")]
    public void TriggerMessage()
    {
        if (alreadyTriggered) return;
        ScanDialogues();

        foreach (var dialog in allDialogues)
        {
            Sprite portrait = null;

            if (dialog.portraitSpeaker == null)
            {
                portrait = dialog.dialogSpeaker.defaultSprite;
            }
            else
            {
                portrait = dialog.portraitSpeaker.portraitSprite;
            }

            DialogueSubtitleUI.instance.QueueDialogue(dialog.Dialogue_Content,
                dialog.dialogSpeaker.name,
                dialog.Dialogue_Timer,
                portrait,
                dialog.dialogAudioClip, priorityLevel: 100,
                isImportant: isImportant,
                shouldOverride: shouldOverride, 
                entryEvent: dialog.OnDialogTriggered);
                
        }

        OnDialogueTriggered?.Invoke();
        alreadyTriggered = true;
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
