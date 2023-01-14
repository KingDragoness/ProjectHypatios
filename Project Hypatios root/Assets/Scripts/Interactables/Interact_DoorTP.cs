using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Interact_DoorTP : InteractableObject
{
    public Transform target;
    public AudioSource interactSound;
    public string interactDescription = "Interact";

    [Button("Interact")]
    public override void Interact()
    {
        Hypatios.Player.transform.position = target.position;
        if (interactSound != null) interactSound.Play();
    }

    public override string GetDescription()
    {
        return interactDescription;
    }

    private void OnDrawGizmos()
    {
        if (target == null) return;

        Gizmos.color = new Color(0.2f, 0.9f, 0.2f, 0.6f);
        var dir = target.position - transform.position;
        int amountArrow = 10;
        float distance = 1f / amountArrow;
        float arrowTimeFinish = 10; //Arrow takes 10 seconds from start to end


        for (int i = 0; i < amountArrow; i++)
        {
            float time = 0f;
            double time1 = 0d;
#if UNITY_EDITOR
            time1 = EditorApplication.timeSinceStartup;
#endif
            float timeR = (float)time1;

            time = Mathf.RoundToInt(timeR * 100f) / 100; //66,15
            time = time % 10; //6.15
            time *= 0.1f; //0.615
            time += timeR - Mathf.FloorToInt(timeR);
            float arrowPos = (i * distance);
            float f1 = arrowPos + (time / arrowTimeFinish);
            if (f1 > 1) f1 -= 1;
            arrowPos = Mathf.Clamp(f1, 0f, 1f);

            DrawArrow.ForGizmo(transform.position, dir, arrowPosition: arrowPos, arrowHeadLength: 1f);
        }

        Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(target.position, 0.5f);
        Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.06f);
        Gizmos.DrawSphere(target.position, 0.5f);

    }


}
