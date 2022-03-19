using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class HitAndDamageProjectile : MonoBehaviour
{

    public bool killByImpact = false;
    public UnityEvent OnHitImpact;

    public GameObject prefabSpawnOnImpact;

    public float Damage = 10;
    public float DamageSpeedOverride = 10;
    public float timerDead = 6f;
    public Rigidbody rigidbody;
    private health PlayerHealth;

    void Start()
    {
        PlayerHealth = FindObjectOfType<health>();
        Destroy(this.gameObject, timerDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DamagePlayer();
            Destroy(this.gameObject);
        }


    }

    public void SpawnSomething()
    {
       var gameObject1 = Instantiate(prefabSpawnOnImpact, transform.position, Quaternion.identity);
        gameObject1.gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (killByImpact)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                return;
            }

            OnHitImpact?.Invoke();
            Destroy(this.gameObject);
        }
    }

    public void DamagePlayer()
    {
        PlayerHealth.takeDamage(Mathf.RoundToInt(Damage), DamageSpeedOverride);
        SpawnIndicator.instance.Spawn(transform);
    }
}
