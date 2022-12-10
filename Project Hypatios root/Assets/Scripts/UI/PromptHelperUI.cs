using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptHelperUI : MonoBehaviour
{

    public Button button;

    private float f = 0.5f;

    public void Update()
    {
        f -= Time.deltaTime;

        if (Input.anyKey && f < 0)
        {
            button.onClick.Invoke();
        }
    }

}
