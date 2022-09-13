using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileChameleon : Enemy
{

    public float health = 55;
    public float rotateSpeed = 100;
    public float moveSpeed = 100;
    public float collisionLimitVelocity = 6f;
    public float initialVelocityForce = 1000;
    public float timer = 10;

    public GameObject explosionPrefab;
    public GameObject explosionHarmlessPrefab;
    public Rigidbody rb;

    private Transform target;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = FindObjectOfType<characterScript>().transform;
        rb.AddForce(transform.forward * initialVelocityForce * rb.mass);
    }

    public override void Attacked(DamageToken token)
    {
        health -= token.damage;
        base.Attacked(token);
        DamageOutputterUI.instance.DisplayText(token.damage);

        if (health < 0)
        {
            Dead(true);
        }

    }

    private void Update()
    {
        var q = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        rb.AddForce(transform.forward * moveSpeed * rb.mass * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer < 0)
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
