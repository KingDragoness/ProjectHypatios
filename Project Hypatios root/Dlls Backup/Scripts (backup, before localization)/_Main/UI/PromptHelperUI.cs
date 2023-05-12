using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PromptHelperUI : MonoBehaviour
{

    public Button button;

    private float f = 0.5f;

    public void Update()
    {
        f -= Time.deltaTime;

        if ((Input.anyKey) && f < 0)
        {
            button.onClick.Invoke();
        }
    }

}
