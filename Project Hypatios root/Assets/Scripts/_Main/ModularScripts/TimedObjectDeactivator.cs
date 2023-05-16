using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDeactivator : MonoBehaviour
{

    [SerializeField] private float timer = 0.1f;
    public bool destroy = false;
    public bool allowRestart = false;
    public bool DEBUG_isDisabledByDefault = false;
    private float currentTimer = 0.1f;

    private bool isStart = false;

    public float Timer 
    { 
        get => timer;
        set
        {
            currentTimer = value;
            timer = value;
        }
    }

    public void OnEnable()
    {
        if (isStart == false && allowRestart == false)
        {
            isStart = true;
        }
        else
        {
            currentTimer = timer;
        }


    }

    private void Start()
    {
        if (DEBUG_isDisabledByDefault)
        {
            currentTimer = timer;
        }
    }

    private void Update()
    {
        if (currentTimer > 0f)
        {
            currentTimer -= Time.deltaTime;
        }
        else
        {
            if (destroy == false)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}
