using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using Sirenix.OdinInspector;

/// <summary>
/// This is for radial menu or UI element on the center
/// </summary>
/// 
[ExecuteInEditMode]
public class UIScalingAuto : MonoBehaviour
{

    public bool useX = false;
    [ShowIf("useX", true, true)] public float ref_X = 1600f;
    [HideIf("useX", true, true)] public float ref_Y = 900f;
    public float defaultScale = 0.75f;
    public CanvasScaler canvasScaler;
    public RectTransform rect;

    private void Update()
    {
        if (canvasScaler == null) return;
        if (rect == null) return;
        float scalerRef = 0f;

        {
            if (useX)
            {
                scalerRef = canvasScaler.referenceResolution.x / ref_X;
            }
            else
            {
                scalerRef = canvasScaler.referenceResolution.y / ref_Y;

            }
        }

        Vector3 scale = new Vector3(defaultScale * scalerRef, defaultScale * scalerRef, defaultScale * scalerRef);


        rect.localScale = scale;
    }

}
