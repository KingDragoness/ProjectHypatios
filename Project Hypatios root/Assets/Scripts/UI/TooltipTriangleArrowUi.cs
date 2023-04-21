using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI.Extensions;

public class TooltipTriangleArrowUi : MonoBehaviour
{
    
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public float minLocalPos = -10f;
    public float maxLocalPos = 10f;
    public Direction direction;
    public RectTransform middlePointRect;

    private RectTransform rectTransform;
    private float yLocPos = 0;
    private float xLocPos = 0;
    private Vector3 mousePos;
    private Vector3 rawLocalMousePos;
    private Vector3 currentPosGlobal;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (direction == Direction.Horizontal)
        {
            yLocPos = rectTransform.localPosition.y;
        }
        else
        {
            xLocPos = rectTransform.localPosition.x;
        }
    }

    private void LateUpdate()
    {
        mousePos = UIExtensionsInputManager.MousePosition;
        var finalLocalPos = middlePointRect.InverseTransformPoint(mousePos);
        currentPosGlobal = middlePointRect.position;
        rawLocalMousePos = finalLocalPos;

        if (direction == Direction.Horizontal)
        {
            if (finalLocalPos.x < minLocalPos) finalLocalPos.x = minLocalPos;
            if (finalLocalPos.x > maxLocalPos) finalLocalPos.x = maxLocalPos;
            finalLocalPos.y = yLocPos;
        }
        else
        {
            //Special for STUPID FUCKING VERTICAL MODE
            float y = mousePos.y - currentPosGlobal.y;
            finalLocalPos.y = y;
            if (finalLocalPos.y < minLocalPos) finalLocalPos.y = minLocalPos;
            if (finalLocalPos.y > maxLocalPos) finalLocalPos.y = maxLocalPos;
            finalLocalPos.x = xLocPos;
        }


        rectTransform.localPosition = finalLocalPos;
    }

}
