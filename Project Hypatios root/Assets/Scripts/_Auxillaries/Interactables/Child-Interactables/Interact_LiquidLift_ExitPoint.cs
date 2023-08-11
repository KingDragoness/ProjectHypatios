using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_LiquidLift_ExitPoint : MonoBehaviour
{

    public Interact_LiquidatorElevatorLift liftScript;
    public Transform exitTransform;
    public TextMesh label_LiftFloor;
    public AnimatorSetBool animatorBoolScript;
    public GameObject goingUp;
    public GameObject goingDown;

    public void CallLift()
    {
        liftScript.CallLift(this);
    }

    public void OpenLift()
    {
        animatorBoolScript.SetBool(true);
    }

    public void CloseLift()
    {
        animatorBoolScript.SetBool(false);
    }

}
