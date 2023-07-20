using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_DialogTrigger : MonoBehaviour
{
    public UnityEvent OnSpeechBubble;

    [Space]

    [TextArea(3, 4)]
    public string Dialogue_Content;
    public DialogSpeaker dialogSpeaker;
    public PortraitSpeaker portraitSpeaker;
    public AudioClip dialogAudioClip;
    public float Dialogue_Timer = 4;
    public bool _isImportant = false;
    [Space]
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

        Sprite portrait = null;

        if (portraitSpeaker == null)
        {
            portrait = dialogSpeaker.defaultSprite;
        }
        else
        {
            portrait = portraitSpeaker.portraitSprite;
        }



        Hypatios.Dialogue.QueueDialogue(Dialogue_Content, dialogSpeaker.name, Dialogue_Timer, portrait, dialogAudioClip, isImportant: _isImportant);
        OnSpeechBubble?.Invoke();

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
