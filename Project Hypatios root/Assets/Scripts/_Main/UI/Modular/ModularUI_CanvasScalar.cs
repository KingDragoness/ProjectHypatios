using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularUI_CanvasScalar : MonoBehaviour
{

    public CanvasScaler canvasScaler;
    public CanvasScaler toCopyFrom;


    private void OnEnable()
    {
        canvasScaler.referenceResolution = toCopyFrom.referenceResolution;
    }

}
