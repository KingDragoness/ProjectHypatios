using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_LiquidatorElevatorLift : MonoBehaviour
{


    public List<Interact_LiquidLift_ExitPoint> allExitPoints = new List<Interact_LiquidLift_ExitPoint>();
    [FoldoutGroup("Lift")] public Transform platformLift;
    [FoldoutGroup("Lift")] public float thresholdArrive = 0.05f;
    [FoldoutGroup("Lift")] public float elevatorSpeed = 5f;
    [FoldoutGroup("Lift")] public AnimatorSetBool myLiftAnimScript;
    [FoldoutGroup("Lift")] public TextMesh label_currentLift;
    [FoldoutGroup("Lift")] public GameObject goingUp;
    [FoldoutGroup("Lift")] public GameObject goingDown;

    public Interact_LiquidLift_ExitPoint currentTarget;

    private void Start()
    {
        currentTarget = allExitPoints[0];
    }

    public void SetLift(int index)
    {
        var liftTarget = allExitPoints[index];
        currentTarget = liftTarget;
    }

    public void CallLift(Interact_LiquidLift_ExitPoint exitPoint)
    {
        var liftTarget = allExitPoints.IndexOf(exitPoint);
        SetLift(liftTarget);

    }

    private void Update()
    {
        if (Time.timeScale <= 0f) return;

        foreach(var lift in allExitPoints)
        {

            if (lift == currentTarget)
            {
                continue;
            }

            lift.CloseLift();

        }

        var step = elevatorSpeed * Time.deltaTime;
        platformLift.transform.position = Vector3.MoveTowards(platformLift.position, currentTarget.exitTransform.position, step);

        //check if arrived
        float dist = Vector3.Distance(platformLift.position, currentTarget.exitTransform.position);

        if (dist < thresholdArrive)
        {
            currentTarget.OpenLift();
            myLiftAnimScript.SetBool(true);
        }
        else
        {
            myLiftAnimScript.SetBool(false);
        }


        UpdateUI();
    }

    private void UpdateUI()
    {
        int currentFloor = 0;
        float nearestDist = 999999;
        int index = 0;
        int index_TargetLift = allExitPoints.IndexOf(currentTarget);
        bool isUp = false;
        bool isEqual = false;

        foreach (var lift in allExitPoints)
        {
            float dist = Vector3.Distance(platformLift.position, lift.exitTransform.position);
            if (dist < nearestDist)
            {
                currentFloor = index;
                nearestDist = dist;
            }
            index++;
        }

        if (currentFloor <= index_TargetLift)
            isUp = true;

        if (currentFloor == index_TargetLift)
            isEqual = true;

        {
            foreach (var lift in allExitPoints)
            {
                lift.label_LiftFloor.text = $"{currentFloor + 1}";
            }

            label_currentLift.text = $"{currentFloor + 1}";
        }

        if (isEqual == false)
        {
            if (isUp)
            {
                LiftGoingUp(true);
                LiftGoingDown(false);
            }
            else
            {
                LiftGoingUp(false);
                LiftGoingDown(true);
            }
        }
        else
        {
            LiftGoingUp(false);
            LiftGoingDown(false);
        }
    }

    public void LiftGoingUp(bool visible)
    {
        if (visible == true)
        {
            foreach (var lift in allExitPoints)
            {
                lift.goingUp.gameObject.SetActive(true);
            }
            goingUp.gameObject.SetActive(true);

        }
        else
        {
            foreach (var lift in allExitPoints)
            {
                lift.goingUp.gameObject.SetActive(false);
            }
            goingUp.gameObject.SetActive(false);
        }
    }

    public void LiftGoingDown(bool visible)
    {
        if (visible == true)
        {
            foreach (var lift in allExitPoints)
            {
                lift.goingDown.gameObject.SetActive(true);
            }
            goingDown.gameObject.SetActive(true);

        }
        else
        {
            foreach (var lift in allExitPoints)
            {
                lift.goingDown.gameObject.SetActive(false);
            }
            goingDown.gameObject.SetActive(false);
        }
    }

}
