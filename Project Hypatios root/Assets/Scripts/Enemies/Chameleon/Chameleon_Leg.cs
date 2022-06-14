using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chameleon_Leg : MonoBehaviour
{

    public Transform targetIK;

    [FoldoutGroup("Target IK")] public float ZLower_Limit = -4f;
    [FoldoutGroup("Target IK")] public float ZUpper_Limit = 4f;
    [FoldoutGroup("Target IK")] public float XLower_Limit = -4f;
    [FoldoutGroup("Target IK")] public float XUpper_Limit = -1f;
    [FoldoutGroup("Target IK")] public float YLower_Limit = -4f;
    [FoldoutGroup("Target IK")] public float YUpper_Limit = -1f;
    [FoldoutGroup("Target IK")] public float XFrequency = 1.0f;
    [FoldoutGroup("Target IK")] public float YFrequency = 1.0f;
    [FoldoutGroup("Target IK")] public float ZFrequency = 1.0f;

    //[FoldoutGroup("Hint Leg")] public   

    private Vector3 startLocalPos;

    private void Start()
    {
        startLocalPos = targetIK.localPosition;
    }

    private void Update()
    {
        targetIK.localPosition = new Vector3(
            CosWave(XFrequency, XLower_Limit + XUpper_Limit),
            SinWave(YFrequency, YLower_Limit + YUpper_Limit),
            SinWave(ZFrequency, ZLower_Limit + ZUpper_Limit));
    }

    public float SinWave(float frequency1, float offset)
    {
        return Mathf.Sin(2 * Mathf.PI * Time.time * frequency1) + (offset);
    }

    public float CosWave(float frequency1, float offset)
    {
        return Mathf.Cos(2 * Mathf.PI * Time.time * frequency1) + (offset);
    }

}
