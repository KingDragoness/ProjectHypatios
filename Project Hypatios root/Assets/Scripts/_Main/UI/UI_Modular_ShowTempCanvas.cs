using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Modular_ShowTempCanvas : MonoBehaviour
{

    public CanvasGroup cg;
    public bool isReverse = false;
    public float opacitySpeed = 5;

    private float _showTime = 1f;

    private void Start()
    {
        cg.alpha = 0;
        _showTime = 0;
    }

    private void Update()
    {
        if (isReverse == false)
        {
            if (_showTime > 0)
            {
                _showTime -= Time.deltaTime;

                if (cg.alpha < 1)
                {
                    cg.alpha += Time.deltaTime * opacitySpeed * 0.1f;
                }
            }
            else if (cg.alpha > 0)
            {
                //hide
                cg.alpha -= Time.deltaTime * opacitySpeed * 0.1f;
            }
        }
        else
        {
            //Debug.Log("test1");
            if (_showTime > 0)
            {
                _showTime -= Time.deltaTime;

                if (cg.alpha > 0)
                {
                    cg.alpha -= Time.deltaTime * opacitySpeed * 0.1f;
                }
            }
            else if (cg.alpha < 1)
            {
                //hide
                cg.alpha += Time.deltaTime * opacitySpeed * 0.1f;
            }
        }
    }

    public void Show(float showTime = 10)
    {
        _showTime = showTime;
    }

}
