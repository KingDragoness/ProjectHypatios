using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMapUI_Legends : MonoBehaviour
{

    public LocalMapUI mapUI;
    public Transform realTransform;
    public Vector3 screenPos;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        screenPos = mapUI.mapCamera.WorldToScreenPoint(realTransform.position);
        rectTransform.anchoredPosition = screenPos * mapUI.GetScalingFactor();
    }

}
