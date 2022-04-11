using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interact_TutorialBubble : MonoBehaviour
{
    public UnityEvent OnSpeechBubble;
    public bool shouldEnqueue = true;
    [TextArea(3, 4)]
    public string Dialogue_Content;
    public string Dialogue_SpeakerName;
    public float Dialogue_Timer = 4;
    [Space]
    public List<Transform> ActivatingArea;

    public Transform player;
    public bool DEBUG_DrawGizmos = false;

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

    public  void TriggerMessage()
    {
        DialogueSubtitleUI.instance.QueueDialogue(Dialogue_Content, Dialogue_SpeakerName, Dialogue_Timer, shouldEnqueue);
        OnSpeechBubble?.Invoke();
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
