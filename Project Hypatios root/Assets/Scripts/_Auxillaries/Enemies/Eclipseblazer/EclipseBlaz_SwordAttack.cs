using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class EclipseBlaz_SwordAttack : EclipseBlaz_AIModule
{

    public enum Mode
    {
        Attack,
        Chasing
    }

    public List<ClipTransition> allAnimationClips = new List<ClipTransition>();
    public float moveSpeed = 5f;
    public float rotationSpeed = 20f;
    public float attackingRange = 12f;
    public Mode currentMode;

    public override void Run()
    {
        var distance = Vector3.Distance(Hypatios.Player.transform.position, transform.position);

        if (distance < attackingRange)
        {
            currentMode = Mode.Attack;
        }
        else
        {
            currentMode = Mode.Chasing;
        }

        if (currentMode == Mode.Attack)
        {
            Mode_Attack();
        }
        else if (currentMode == Mode.Chasing)
        {
            Mode_Chasing();
        }

    }

    private void Mode_Attack()
    {
        //get time 
        var clip = allAnimationClips[Random.Range(0, allAnimationClips.Count)];
        var animLength = clip.Clip.length;
    
        
    }

    private void Mode_Chasing()
    {

        var step = moveSpeed * Time.deltaTime;
        Vector3 _currentPosition = Hypatios.Player.transform.position;

        var lookPos = _currentPosition - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        eclipseblazer.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        eclipseblazer.transform.position = Vector3.MoveTowards(transform.position, _currentPosition, step);
    }

}
