using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    

    // Update is called once per frame
    public void SlowMo()
    {
        if (Time.timeScale > 0f)
        {
            Time.timeScale -= Time.deltaTime;
        } 
    }

    public void ReturnTime()
    {
        Time.timeScale = 1f;
    }
}
