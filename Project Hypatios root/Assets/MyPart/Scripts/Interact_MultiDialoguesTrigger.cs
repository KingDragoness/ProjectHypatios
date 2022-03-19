using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_MultiDialoguesTrigger : MonoBehaviour
{

    public List<MultiDialogues_SingleSpeech> allDialogues;
    public List<Transform> ActivatingArea;

    public Transform player;
    public bool DEBUG_DrawGizmos = false;

    [SerializeField] private bool alreadyTriggered = false;

    void Start()
    {

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
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, t.localScale);
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

    public void TriggerMessage()
    {
        if (alreadyTriggered) return;

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
                dialog.Dialogue_Timer, true,
                portrait,
                dialog.dialogAudioClip);
        }

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
