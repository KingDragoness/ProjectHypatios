using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EclipseBlaz_Patrol : EclipseBlaz_AIModule
{

    public RandomSpawnArea patrolArea;
    public float moveSpeed = 5f;
    public float rotationSpeed = 20f;
    public float clockTimer = 5f;
    public float thresholdDistancePatrol = 5f;

    private Vector3 _currentPosition = new Vector3();
    private float _timer = 5f;

    private void Start()
    {
        _currentPosition = Hypatios.Player.transform.position;
    }

    public override void Run()
    {
        var step = moveSpeed * Time.deltaTime;
        var distance = Vector3.Distance(_currentPosition, transform.position);
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            ChangePosition();
            _timer = clockTimer;
        }

        if (distance < thresholdDistancePatrol)
        {
            ChangePosition();
        }

        var lookPos = _currentPosition - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        eclipseblazer.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        eclipseblazer.transform.position = Vector3.MoveTowards(transform.position, _currentPosition, step);
    }

    private void ChangePosition()
    {
        _currentPosition = patrolArea.GetAnyPositionInsideBox();

    }

}
