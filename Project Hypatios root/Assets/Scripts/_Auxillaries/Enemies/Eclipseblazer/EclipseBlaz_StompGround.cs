using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EclipseBlaz_StompGround : EclipseBlaz_AIModule
{

    public float timeToTriggerJump = 1.1f;
    public float sendPlayerUpForce = 2000f;

    private float _timeToTrigger = 1f;
    private bool _hasTriggered = false;

    public override void OnEnterState()
    {
        _timeToTrigger = timeToTriggerJump;
        _hasTriggered = false;
        base.OnEnterState();
    }

    public override void Run()
    {
        _timeToTrigger -= Time.deltaTime;

        if (_timeToTrigger <= 0f && _hasTriggered == false)
        {
            TriggerSendUp();
            _hasTriggered = true;
        }

        base.Run();
    }

    public void TriggerSendUp()
    {
        Hypatios.Player.rb.AddForce(Vector3.up * sendPlayerUpForce, ForceMode.VelocityChange);
    }
}
