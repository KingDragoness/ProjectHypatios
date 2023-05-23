using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class FavoriteRadialCategoryButton : MonoBehaviour
{

    //54f
    public float minRadian = -27f;
    public float maxRadian = 27f;
    public ItemInventory.SubiconCategory subCategory;
    public Animator animator;

    public void UpdateButton(float _angle)
    {
        if (minRadian < _angle && maxRadian > _angle)
        {
            animator.ResetTrigger("Highlighted");
            animator.ResetTrigger("Normal");
            animator.SetTrigger("Highlighted");
        }
        else
        {
            animator.ResetTrigger("Normal");
            animator.ResetTrigger("Highlighted");
            animator.SetTrigger("Normal");
        }
    }

    public bool IsSelected (float _angle)
    {
        if (minRadian < _angle && maxRadian > _angle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
