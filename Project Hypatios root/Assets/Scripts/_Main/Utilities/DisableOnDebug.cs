using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnDebug : MonoBehaviour
{
    private void Start()
    {
        if (Time.timeSinceLevelLoad < 1)
        {
            gameObject.SetActive(false);
        }
    }
}
