using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ExplosionAreaEffect : MonoBehaviour
{
    public Transform AreaTransform;
    public UnityEvent OnHitTrigger;
    public BaseStatusEffectObject statusEffect;
    public float time = 1;
    public bool DEBUG_DrawGizmos = false;
    public bool isTriggerOnStart = true;
    public bool isReaperDescription = false;
    [ShowIf("isReaperDescription")] public string reaperDescription = "";
    [ShowIf("isReaperDescription")] public float reaperPromptTime = 5f;

    private void OnDrawGizmos()
    {

        if (DEBUG_DrawGizmos == false)
        {
            return;
        }

        Transform t = AreaTransform;
        if (t == null) return;

        Gizmos.matrix = t.localToWorldMatrix;
        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.5f);
        Gizmos.DrawWireCube(Vector3.zero, t.localScale);
        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.04f);
        Gizmos.DrawCube(Vector3.zero, t.localScale);

        {
            Vector3 v1 = t.localScale / 2f;
            Vector3 v2 = -t.localScale / 2f;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v1);
        }

    }

    private void OnEnable()
    {
        if (isTriggerOnStart) StartCoroutine(LateTriggerCheck());
    }

    public IEnumerator LateTriggerCheck()
    {
        yield return new WaitForSeconds(0.1f);
        CheckPlayerInArea();
    }

    private void CheckPlayerInArea()
    {
        if (IsInsideOcclusionBox(AreaTransform, Hypatios.Player.transform.position))
        {
            AddStatusEffect();
        }
    }

    public void AddStatusEffect()
    {
        if (Hypatios.Player.IsStatusEffectGroup(statusEffect)) return;
        statusEffect.AddStatusEffectPlayer(time);
        OnHitTrigger?.Invoke();
        DeadDialogue.PromptNotifyMessage_Mod(reaperDescription, reaperPromptTime);
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
