using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Modular_CanvasOpacityController : MonoBehaviour
{

    public bool isVisible = true;
    [SerializeField] private float transitionTime = 1.5f;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup.alpha = 0;

        if (isVisible == false)
            canvasGroup.alpha = 1;
    }

    private void Update()
    {
        if (canvasGroup.alpha >= 0 && isVisible == false) canvasGroup.alpha -= Time.deltaTime * (1/transitionTime);
        if (canvasGroup.alpha <= 1 && isVisible == true) canvasGroup.alpha += Time.deltaTime * (1/transitionTime);
    }

}
