using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LocalMapUI : MonoBehaviour
{

    public Camera mapCamera;
    public CanvasScaler canvasScaler;

    public float GetScalingFactor()
    {
        float screen_X = Screen.width;
        float screen_Y = Screen.height;

        float factor = canvasScaler.referenceResolution.y / screen_Y;

        return factor;
    }

}
