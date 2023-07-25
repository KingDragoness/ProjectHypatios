using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetBool : MonoBehaviour
{

    public string boolParamName = "Opened";
    public Animator animator;

    private bool cached_State = false;
    private bool everSet = false;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        if (everSet)
        {
            animator.SetBool(boolParamName, cached_State);
        }
    }

    public void SetBool(bool state)
    {
        everSet = true;
        cached_State = state;
        animator.SetBool(boolParamName, state);
    }

    public void ToggleBool()
    {
        SetBool(!cached_State);
    }

}
