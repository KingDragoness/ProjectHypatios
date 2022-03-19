using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetBool : MonoBehaviour
{

    public string boolParamName = "Opened";
    public Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void SetBool(bool state)
    {
        animator.SetBool(boolParamName, state);
    }

}
