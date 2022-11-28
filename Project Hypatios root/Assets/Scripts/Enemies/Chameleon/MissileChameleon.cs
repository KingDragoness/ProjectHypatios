using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MissileChameleon : EnemyScript
{

    public float rotateSpeed = 100;
    public float moveSpeed = 100;
    public float collisionLimitVelocity = 6f;
    public float initialVelocityForce = 1000;
    public float timer = 10;

    public GameObject explosionPrefab;
    public GameObject explosionHarmlessPrefab;
    public Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (currentTarget == null) currentTarget = Hypatios.Player;
        rb.AddForce(transform.forward * initialVelocityForce * rb.mass);
    }

    public void OverrideTarget(Entity t, Alliance currentAlliance)
    {
        currentTarget = t;
        Stats.MainAlliance = currentAlliance;
    }

    public override void Attacked(DamageToken token)
    {
        if (token.origin == DamageToken.DamageOrigin.Enemy) return;

        Stats.CurrentHitpoint -= token.damage;
        base.Attacked(token);
        DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint < 0)
        {
            Dead(true);
        }

    }

    private void Update()
    {
        var q = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        rb.AddForce(transform.forward * moveSpeed * rb.mass * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer < 0 | currentTarget == null)
        {
            Dead(true);
        }

    }

    private void Dead(bool harmless = false)
    {
        GameObject explosion = explosionPrefab.gameObject;

        if (harmless)
        {
            explosion = explosionHarmlessPrefab.gameObject;
        }
        GameObject prefab1 = Instantiate(explosion, transform.position, Quaternion.identity);
        prefab1.gameObject.SetActive(true);

        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > collisionLimitVelocity)
        {
            Dead();
        }
    }

}
