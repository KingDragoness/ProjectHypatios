using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class Tween_MovePosScript : MonoBehaviour
{

    public GameObject targetObject;
    public Transform target;
    public LeanTweenType leanType;

    public void MoveTween()
    {

        LeanTween.move(targetObject, target, 2f).setEase(leanType);

    }

}
