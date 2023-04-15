using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyTest_Swarm : EnemyScript
{

    public Vector3 targetSwarmPos;
    public float moveForce = 100f;
    public float distChangeTarget = 5f;
    public float distFindToTarget = 10f;
    public float speedRotate = 10;
    public float errorMargin = 0.1f;
    public bool isTargetPlayer = false;
    [InfoBox("Controlled by Microbots Controller")] public bool avoidanceLeft = false;
    public bool avoidanceRight = false;

    private Rigidbody rb;



  

    public override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        base.Awake();
    }

    private void Start()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        Hypatios.Microbot.AllSwarmDrones.Add(this);
    }

    public override void OnDestroy()
    {
        Hypatios.Microbot.AllSwarmDrones.Remove(this);
        base.OnDestroy();
    }

    public float CooldownChangeDir = 6f;
    private float _timerChangeDir = 6f;
    private Vector3 offsetPos = new Vector3();

    private void Update()
    {
        if (isAIEnabled == false) return;

        _timerChangeDir -= Time.deltaTime;

        if (_timerChangeDir < 0f)
        {
            offsetPos.x = Random.Range(-1, 1f); 
            offsetPos.y = Random.Range(-1, 1f);
            offsetPos.z = Random.Range(-1, 1f);

            PickRandomBoidPosition();
            _timerChangeDir = CooldownChangeDir + Random.Range(-1,2f);
        }

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (isTargetPlayer && currentTarget != null)
        {
            var targetPos = currentTarget.transform.position + offsetPos;
            if (avoidanceLeft)
                targetPos = transform.position + -transform.right * 10f;
            if (avoidanceRight)
                targetPos = transform.position + transform.right * 10f;
            targetSwarmPos = targetPos;
            LookAtPlayer();

        }
    }

    private void LookAtPlayer()
    {
        Vector3 target = targetSwarmPos;
        target.x += Random.Range(-errorMargin, errorMargin);
        target.y += Random.Range(-errorMargin, errorMargin);
        target.z += Random.Range(-errorMargin, errorMargin);
        var q = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isAIEnabled == false) return;

        MoveBoid();
    }

    private void MoveBoid()
    {
        float dist = Vector3.Distance(transform.position, targetSwarmPos);
        rb.AddForce(transform.forward * moveForce * Time.deltaTime);

        if (dist < distChangeTarget)
            PickRandomBoidPosition();
    }

    private void PickRandomBoidPosition()
    {
        LayerMask lm = Hypatios.Player.Weapon.defaultLayerMask;
        lm &= ~(1 << LayerMask.NameToLayer("Enemy"));
        lm |= ~(1 << LayerMask.NameToLayer("Player"));
        var randomDir = Random.insideUnitSphere.normalized;
        var hit = Hypatios.Enemy.GetHit(eyeLocation.transform.position, randomDir, distFindToTarget, lm);

        if (hit.collider != null)
        {
            targetSwarmPos = hit.point + hit.normal * 1f;
        }
        else
        {
            targetSwarmPos = hit.point;
        }


    }

    public override void Die()
    {

        if (Stats.IsDead)
            return;

        OnSelfKilled?.Invoke();
        OnDied?.Invoke();
        Destroy(gameObject);

        Stats.IsDead = true;
    }

    public override void Attacked(DamageToken token)
    {
        _lastDamageToken = token;

        hasSeenPlayer = true;
        Stats.CurrentHitpoint -= token.damage;

        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * -1 * 100 * token.repulsionForce);
        }
        else
        {
            transform.position += Vector3.back * 0.05f * token.repulsionForce;
        }

        if (Stats.IsDead == false)
            if (token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally) DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }


    }

}
