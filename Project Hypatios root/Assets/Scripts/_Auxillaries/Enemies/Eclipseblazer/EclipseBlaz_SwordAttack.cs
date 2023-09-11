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
    public GameObject sword;
    public float moveSpeed = 5f;
    public float rotationSpeed = 20f;
    public float attackingRange = 12f;
    public float yPos = 0;
    public Mode currentMode;

    private Mode previousMode;

    private void Start()
    {
        sword.gameObject.SetActive(false);
    }

    public override void Run()
    {
        var distance = Vector3.Distance(Hypatios.Player.transform.position, transform.position);
        _animationTime -= Time.deltaTime;

        if (distance < attackingRange)
        {
            currentMode = Mode.Attack;
        }
        else
        {
            if (_animationTime <= 0f)
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

        if (previousMode != currentMode)
        {
            OnChangeMode();
        }
        previousMode = currentMode;
    }

    public override void OnEnterState()
    {
        sword.gameObject.SetActive(true);
        _animationTime = 0f;
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        sword.gameObject.SetActive(false);

        base.OnExitState();
    }

    private void OnChangeMode()
    {
        if (currentMode == Mode.Chasing)
        {
            eclipseblazer.AnimatorPlayer.PlayAnimation(idleAnimation, 1f);
        }
        else if (currentMode == Mode.Attack)
        {

        }
    }

    private float _animationTime = 1f;

    private void Mode_Attack()
    {
        //get time 
        var clip = allAnimationClips[Random.Range(0, allAnimationClips.Count)];

        var animLength = clip.MaximumDuration;


        if (_animationTime <= 0f)
        {
            float startPoint = clip.NormalizedStartTime;
            float endPoint = clip.SerializedEvents.GetNormalizedEndTime();
            float duration = animLength * (endPoint - startPoint);
            eclipseblazer.AnimatorPlayer.PlayAnimation(clip, 0.2f);

            //Debug.Log($"{startPoint}:{endPoint} | {animLength} = {duration}");
            _animationTime = duration;
        }
    }

    private void Mode_Chasing()
    {

        var step = moveSpeed * Time.deltaTime;
        Vector3 _currentPosition = Hypatios.Player.transform.position;
        _currentPosition.y = yPos;

        var lookPos = _currentPosition - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        eclipseblazer.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        eclipseblazer.transform.position = Vector3.MoveTowards(transform.position, _currentPosition, step);
    }

}
