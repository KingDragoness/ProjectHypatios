using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EclipseBlaz_ChainAttack : EclipseBlaz_AIModule
{

    public float timeToTriggerJump = 1.1f;
    public float dragPlayerForce = 300f;
    public float anchorTravelSpeed = 30f;
    public float thresholdDistAttachPlayer = 2f;
    public float distanceIncreaseWeightFactor = 200f;
    public float distanceNearPlayerReleaseHook = 20f;
    public Transform pivotStart;
    public Transform pivotEnd;
    public LineRenderer chainLine;
    public GameObject anchorObject;
    public bool isDEBUGHook = false;

    private float _timeToTrigger = 1f;
    [SerializeField] private bool _hasHooked = false;

    public override int GetWeight()
    {
        int netWeight = base.GetWeight();
        float playerDist = GetPlayerDistance();

        if (playerDist > distanceIncreaseWeightFactor)
        {
            netWeight *= 100;
        }
        if (playerDist > distanceNearPlayerReleaseHook && _hasHooked)
        {
            netWeight *= 50;
        }

        return netWeight;
    }

    public float GetPlayerDistance()
    {
        float dist = Vector3.Distance(Hypatios.Player.transform.position, transform.position);
        return dist;
    }

    public override void OnEnterState()
    {
        _timeToTrigger = timeToTriggerJump;
        anchorObject.gameObject.SetActive(true);
        chainLine.gameObject.SetActive(true);
        _hasHooked = false;
        //launch pivot
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        anchorObject.gameObject.SetActive(false);
        chainLine.gameObject.SetActive(false);
        base.OnExitState();
    }

    public override void Run()
    {
        var dir_PivotEnd = Hypatios.Player.transform.position - pivotEnd.transform.position;
        Vector3[] pos = new Vector3[2];
        pos[0] = pivotStart.position;
        pos[1] = pivotEnd.position;

        //travel hook
        pivotEnd.Translate(dir_PivotEnd * anchorTravelSpeed * Time.deltaTime);
        anchorObject.transform.position = pivotEnd.position;
        chainLine.SetPositions(pos);

        //if distance near, lock!
        float dist_anchor = Vector3.Distance(Hypatios.Player.transform.position, pivotEnd.position);

        if (dist_anchor < thresholdDistAttachPlayer)
        {
            _hasHooked = true;
        }

        if (_hasHooked)
        {
            DragPlayer();
        }

        base.Run();
    }

    private void Update()
    {
        if (isDEBUGHook)
        {
            DragPlayer();
        }
    }

    public void DragPlayer()
    {
        var dir = transform.position - Hypatios.Player.transform.position;

        Hypatios.Player.rb.AddForce(dir * dragPlayerForce * Time.deltaTime, ForceMode.Force);
    }
}
